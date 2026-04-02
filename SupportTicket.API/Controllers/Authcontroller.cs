using global::SupportTicket.API.DTOs;
using global::SupportTicket.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SupportTicket.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var token = await _authService.RegisterAsync(dto);

        if (token is null)
            return Conflict(new { message = "Username already exists." });

        return CreatedAtAction(nameof(Register), new { token });
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto);

        if (token is null)
            return Unauthorized(new { message = "Invalid username or password." });

        return Ok(new { token });
    }
}
