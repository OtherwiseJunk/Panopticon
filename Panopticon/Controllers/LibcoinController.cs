using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Panopticon.Data.Contexts;
using Panopticon.Data.Interfaces;
using Panopticon.Enums;
using Panopticon.Models.Core;
using Panopticon.Models.Libcoin;

namespace Panopticon.Controllers;

[ApiController]
[Route("/libcoin")]
public class LibcoinController(IApiKeyService apiKeyService, ILibcoinService libcoinService, IDbContextFactory<PanopticonContext> contextFactory) : ControllerBase
{
    private readonly IApiKeyService _apiKeyService = apiKeyService;
    private readonly ILibcoinService _libcoinService = libcoinService;
    private readonly IDbContextFactory<PanopticonContext> _contextFactory = contextFactory;
    
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

        using PanopticonContext context = _contextFactory.CreateDbContext();
        var balancesQuery = _libcoinService.GetAllLibcoinBalances(context).OrderByDescending((b) => b.Balance);
    
        var pagedBalances = PageResults(balancesQuery, pageNumber, pageSize).ToList();
        return Ok(pagedBalances);
    }

    [HttpGet("transactions")]
    public IActionResult GetAllLibcoinTransactions(int pageNumber = 1, int pageSize = 10)
    {
        var apiKey = Request.Headers["ApiKey"];
        if (!HasReadAllPermission(apiKey!))
        {
            return Unauthorized();
        }

        using PanopticonContext context = _contextFactory.CreateDbContext();
        var transactionsQuery = _libcoinService.GetAllLibcoinTransactions(context).OrderByDescending((t) => t.TransactionDate);

        var pagedTransactions = PageResults(transactionsQuery, pageNumber, pageSize).ToList();
        pagedTransactions = MapApiKeyToDeveloperName(pagedTransactions).ToList();
        
        return Ok(pagedTransactions);
    }

    [HttpGet("transactions/{userId}")]
    public IActionResult GetAllLibcoinTransactionsForUser(string userId, int pageNumber = 1, int pageSize = 10)
    {
        var apiKey = Request.Headers["ApiKey"];
        if (!(HasReadAllPermission(apiKey!)|| !(apiKey == userId && HasReadPersonalPermission(apiKey!))))
        {
            return Unauthorized();
        }

        using PanopticonContext context = _contextFactory.CreateDbContext();
        var transactionsQuery = _libcoinService.GetAllLibcoinTransactionsForUser(context, userId).OrderByDescending((t) => t.TransactionDate);

        var pagedTransactions = PageResults(transactionsQuery, pageNumber, pageSize).ToList();
        pagedTransactions = MapApiKeyToDeveloperName(pagedTransactions);

        return Ok(pagedTransactions);
    }

    [HttpGet("transactions/{userId}/sent")]
    public IActionResult GetAllLibcoinTransactionsSentByUser(string userId, int pageNumber = 1, int pageSize = 10)
    {
        var apiKey = Request.Headers["ApiKey"];
        if (!(HasReadAllPermission(apiKey!)|| !(apiKey == userId && HasReadPersonalPermission(apiKey!))))
        {
            return Unauthorized();
        }

        using PanopticonContext context = _contextFactory.CreateDbContext();
        var transactionsQuery = _libcoinService.GetSentLibcoinTransactionsForUser(context, userId)
            .OrderByDescending(t => t.TransactionDate);

        var pagedTransactions = PageResults(transactionsQuery, pageNumber, pageSize).ToList();
        pagedTransactions = MapApiKeyToDeveloperName(pagedTransactions);

        return Ok(pagedTransactions);
    }

    [HttpGet("transactions/{userId}/received")]
    public IActionResult GetAllLibcoinTransactionsReceivedByUser(string userId, int pageNumber = 1, int pageSize = 10)
    {
        var apiKey = Request.Headers["ApiKey"];
        if (!(HasReadAllPermission(apiKey!) || !(apiKey == userId && HasReadPersonalPermission(apiKey!))))
        {
            return Unauthorized();
        }

        using PanopticonContext context = _contextFactory.CreateDbContext();
        var transactionsQuery = _libcoinService.GetReceivedLibcoinTransactionsForUser(context, userId)
            .OrderByDescending(t => t.TransactionDate);

        var pagedTransactions = PageResults(transactionsQuery, pageNumber, pageSize).ToList();
        pagedTransactions = MapApiKeyToDeveloperName(pagedTransactions);

        return Ok(pagedTransactions);
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
        if (!transactions.Any())
        {
            return transactions;
        }

        var apiKeyStringsToResolve = new HashSet<string>();
        foreach (var transaction in transactions)
        {
            if (!ulong.TryParse(transaction.SendingUser, out _))
            {
                apiKeyStringsToResolve.Add(transaction.SendingUser);
            }
            if (!ulong.TryParse(transaction.ReceivingUser, out _))
            {
                apiKeyStringsToResolve.Add(transaction.ReceivingUser);
            }
        }

        if (!apiKeyStringsToResolve.Any())
        {
            return transactions;
        }

        var developerNamesMap = _apiKeyService.GetDeveloperNamesForKeys(apiKeyStringsToResolve);

        foreach (var transaction in transactions)
        {
            if (!ulong.TryParse(transaction.SendingUser, out _))
            {
                transaction.SendingUser = developerNamesMap.TryGetValue(transaction.SendingUser, out var devName) ? devName : "Unknown";
            }
            if (!ulong.TryParse(transaction.ReceivingUser, out _))
            {
                transaction.ReceivingUser = developerNamesMap.TryGetValue(transaction.ReceivingUser, out var devName) ? devName : "Unknown";
            }
        }

        return transactions;
    }
    
    private IQueryable<T> PageResults<T>(IOrderedQueryable<T> query, int pageNumber, int pageSize)
    {
        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
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