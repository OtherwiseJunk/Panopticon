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
        var apiKey = Request.Headers["ApiKey"];
        if (!(HasReadAllPermission(apiKey!)|| !(apiKey == userId && HasReadPersonalPermission(apiKey!))))
        {
            return Unauthorized();
        }
        
        double balance = _libcoinService.GetLibcoinBalance(userId);
        return Ok(balance);
    }

    [HttpGet("balances")]
    public IActionResult GetAllLibcoinBalances(int pageNumber = 1, int pageSize = 10)
    {
        var apiKey = Request.Headers["ApiKey"];
        if (!HasReadAllPermission(apiKey!))
        {
            return Unauthorized();
        }        
        
        var balances = _libcoinService.GetAllLibcoinBalances().OrderByDescending((b) => b.Balance).ToList();
        return Ok(PageResults(balances, pageNumber, pageSize));
    }

    [HttpGet("transactions")]
    public IActionResult GetAllLibcoinTransactions(int pageNumber = 1, int pageSize = 10)
    {
        var apiKey = Request.Headers["ApiKey"];
        if (!HasReadAllPermission(apiKey!))
        {
            return Unauthorized();
        }
        
        var transactions = _libcoinService.GetAllLibcoinTransactions().OrderByDescending((t) => t.TransactionDate).ToList();
        transactions = MapApiKeyToDeveloperName(transactions);
        
        return Ok(PageResults(transactions, pageNumber, pageSize));
    }

    [HttpGet("transactions/{userId}")]
    public IActionResult GetAllLibcoinTransactionsForUser(string userId, int pageNumber = 1, int pageSize = 10)
    {
        var apiKey = Request.Headers["ApiKey"];
        if (!(HasReadAllPermission(apiKey!)|| !(apiKey == userId && HasReadPersonalPermission(apiKey!))))
        {
            return Unauthorized();
        }

        var transactions = _libcoinService.GetAllLibcoinTransactionsForUser(userId).OrderByDescending((t) => t.TransactionDate).ToList();
        transactions = MapApiKeyToDeveloperName(transactions);
        
        return Ok(PageResults(transactions, pageNumber, pageSize));
    }

    [HttpGet("transactions/{userId}/sent")]
    public IActionResult GetAllLibcoinTransactionsSentByUser(string userId, int pageNumber = 1, int pageSize = 10)
    {
        var apiKey = Request.Headers["ApiKey"];
        if (!(HasReadAllPermission(apiKey!)|| !(apiKey == userId && HasReadPersonalPermission(apiKey!))))
        {
            return Unauthorized();
        }

        var transactions = _libcoinService.GetSentLibcoinTransactionsForUser(userId).OrderByDescending((t) => t.TransactionDate).ToList();
        transactions = MapApiKeyToDeveloperName(transactions);
        
        return Ok(PageResults(transactions, pageNumber, pageSize));
    }

    [HttpGet("transactions/{userId}/received")]
    public IActionResult GetAllLibcoinTransactionsReceivedByUser(string userId, int pageNumber = 1, int pageSize = 10)
    {
        var apiKey = Request.Headers["ApiKey"];
        if (!(HasReadAllPermission(apiKey!) || !(apiKey == userId && HasReadPersonalPermission(apiKey!))))
        {
            return Unauthorized();
        }

        var transactions = _libcoinService.GetReceivedLibcoinTransactionsForUser(userId)
            .OrderByDescending((t) => t.TransactionDate).ToList();
        transactions = MapApiKeyToDeveloperName(transactions);

        return Ok(PageResults(transactions, pageNumber, pageSize));
    }

    [HttpPost("send")]
    public IActionResult SendLibcoin([FromBody] SendLibcoinRequest request)
    {
        var apiKey = Request.Headers["ApiKey"];
        if (!( HasBrokerPermission(apiKey!) || (apiKey == request.SendingUserId && HasSendPermission(apiKey!))))
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
        if (!HasDeductPermission(apiKey!))
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
    private bool HasReadPersonalPermission(string apiKey)
    {
        return _apiKeyService.HasPermission(apiKey, ApiPermission.LibcoinReadPersonal);
    }
    private bool HasReadAllPermission(string apiKey)
    {
        return _apiKeyService.HasPermission(apiKey, ApiPermission.LibcoinReadAll);
    }
    private bool HasBrokerPermission(string apiKey)
    {
        return _apiKeyService.HasPermission(apiKey, ApiPermission.LibcoinBroker);
    }
    private bool HasDeductPermission(string apiKey)
    {
        return _apiKeyService.HasPermission(apiKey, ApiPermission.LibcoinDeduct);
    }
    private bool HasSendPermission(string apiKey)
    {
        return _apiKeyService.HasPermission(apiKey, ApiPermission.LibcoinSend);
    }
}