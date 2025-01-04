using Microsoft.AspNetCore.Mvc;
using Panopticon.Data.Interfaces;

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
}