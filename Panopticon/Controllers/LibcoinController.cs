using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Panopticon.Data.Interfaces;
using Panopticon.Enums;
using Panopticon.Models.Core;
using Panopticon.Models.Libcoin;

namespace Panopticon.Controllers;

[ApiController]
[Route("/libcoin")]
public class LibcoinController(IApiKeyService apiKeyService, ILibcoinService libcoinService) : ControllerBase
{
    private readonly IApiKeyService _apiKeyService = apiKeyService;
    private readonly ILibcoinService _libcoinService = libcoinService;
    
    [HttpGet("{userId}")]
    public IActionResult GetLibcoinBalance(string userId)
    {
        double balance = _libcoinService.GetLibcoinBalance(userId);
        return Ok(balance);
    }

    [HttpGet("transactions")]
    public IActionResult GetAllLibcoinTransactions(int pageNumber = 1, int pageSize = 10)
    {
        var apiKey = Request.Headers["ApiKey"];
        if (!_apiKeyService.HasPermission(apiKey!, ApiPermission.LibcoinReadAll))
        {
            return Unauthorized();
        }
        
        var transactions = _libcoinService.GetAllLibcoinTransactions();
        transactions = MapApiKeyToDeveloperName(transactions);
        
        return Ok(PageResults(transactions, pageNumber, pageSize));
    }

    [HttpGet("transactions/{userId}")]
    public IActionResult GetAllLibcoinTransactionsForUser(string userId, int pageNumber = 1, int pageSize = 10)
    {
        var apiKey = Request.Headers["ApiKey"];
        if (!_apiKeyService.HasPermission(apiKey!, ApiPermission.LibcoinReadAll) || !(apiKey == userId && _apiKeyService.HasPermission(apiKey!, ApiPermission.LibcoinReadPersonal)))
        {
            return Unauthorized();
        }

        var transactions = _libcoinService.GetAllLibcoinTransactionsForUser(userId);
        transactions = MapApiKeyToDeveloperName(transactions);
        
        return Ok(PageResults(transactions, pageNumber, pageSize));
    }

    [HttpGet("transactions/{userId}/sent")]
    public IActionResult GetAllLibcoinTransactionsSentByUser(string userId, int pageNumber = 1, int pageSize = 10)
    {
        var apiKey = Request.Headers["ApiKey"];
        if (!_apiKeyService.HasPermission(apiKey!, ApiPermission.LibcoinReadAll) || !(apiKey == userId && _apiKeyService.HasPermission(apiKey!, ApiPermission.LibcoinReadPersonal)))
        {
            return Unauthorized();
        }

        var transactions = _libcoinService.GetSentLibcoinTransactionsForUser(userId);
        transactions = MapApiKeyToDeveloperName(transactions);
        
        return Ok(PageResults(transactions, pageNumber, pageSize));
    }
    
    [HttpGet("transactions/{userId}/received")]
    public IActionResult GetAllLibcoinTransactionsReceivedByUser(string userId, int pageNumber = 1, int pageSize = 10)
    {
        var apiKey = Request.Headers["ApiKey"];
        if (!_apiKeyService.HasPermission(apiKey!, ApiPermission.LibcoinReadAll) || !(apiKey == userId && _apiKeyService.HasPermission(apiKey!, ApiPermission.LibcoinReadPersonal)))
        {
            return Unauthorized();
        }

        var transactions = _libcoinService.GetReceivedLibcoinTransactionsForUser(userId);
        transactions = MapApiKeyToDeveloperName(transactions);
        
        return Ok(PageResults(transactions, pageNumber, pageSize));
    }

    [HttpPost("send")]
    public IActionResult SendLibcoin([FromBody] SendLibcoinRequest request)
    {
        var apiKey = Request.Headers["ApiKey"];
        var isBroker = _apiKeyService.HasPermission(apiKey!, ApiPermission.LibcoinBroker);
        var isSendingUser = apiKey == request.SendingUserId;
        var isAuthorizedToSend = isSendingUser && _apiKeyService.HasPermission(apiKey!, ApiPermission.LibcoinSend);
        if (!(isBroker || isAuthorizedToSend))
        {
            return Unauthorized();
        }
        
        try
        {
            _libcoinService.SendLibcoin(request.SendingUserId, request.ReceivingUserId, request.Amount, request.Message);
            _apiKeyService.CreateApiTransaction(new ApiTransaction
            {
                ApiKey = apiKey!,
                Type = ApiTransactionType.Libcoin,
                TransactionData = JsonSerializer.Serialize(request)
            });

            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [HttpPost("grant")]
    public IActionResult GrantLibcoin([FromBody] GrantLibcoinRequest request)
    {
        var apiKey = Request.Headers["ApiKey"];
        if (!_apiKeyService.HasPermission(apiKey!, ApiPermission.LibcoinGrant))
        {
            return Unauthorized();
        }

        try
        {
            _libcoinService.GrantLibcoin(request.UserId, request.Amount, apiKey!, request.Message);
            _apiKeyService.CreateApiTransaction(new ApiTransaction
            {
                ApiKey = apiKey!,
                Type = ApiTransactionType.Libcoin,
                TransactionData = JsonSerializer.Serialize(request)
            });   
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        
        return Ok();
    }
    [HttpPost("deduct")]
    public IActionResult DeductLibcoin([FromBody] DeductLibcoinRequest request)
    {
        var apiKey = Request.Headers["ApiKey"];
        if (!_apiKeyService.HasPermission(apiKey!, ApiPermission.LibcoinDeduct))
        {
            return Unauthorized();
        }

        try
        {
            _libcoinService.DeductLibcoin(request.UserId, request.Amount, apiKey!, request.Message);
            _apiKeyService.CreateApiTransaction(new ApiTransaction
            {
                ApiKey = apiKey!,
                Type = ApiTransactionType.Libcoin,
                TransactionData = JsonSerializer.Serialize(request)
            });   
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        
        return Ok();
    }
    

    private List<LibcoinTransaction> MapApiKeyToDeveloperName(List<LibcoinTransaction> transactions)
    {
        foreach (var transaction in transactions)
        {
            if (!ulong.TryParse(transaction.SendingUser, out _))
            {
                transaction.SendingUser = _apiKeyService.TryGetApiKey(transaction.SendingUser)?.DeveloperName ?? "Unknown";
            }
            if (!ulong.TryParse(transaction.ReceivingUser, out _))
            {
                transaction.ReceivingUser = _apiKeyService.TryGetApiKey(transaction.ReceivingUser)?.DeveloperName ?? "Unknown";
            }
        }

        return transactions;
    }
    
    private List<T> PageResults<T>(List<T> results, int pageNumber, int pageSize)
    {
        return results.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
    }
}