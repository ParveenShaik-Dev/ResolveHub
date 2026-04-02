using System.Security.Claims;
using global::SupportTicket.API.Data;
using global::SupportTicket.API.DTOs;
using global::SupportTicket.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace SupportTicket.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]  // ← only Admin can access
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/admin/agents — get all agents
    [HttpGet("agents")]
    public IActionResult GetAgents()
    {
        var agents = _context.Users
            .Where(u => u.Role == "Agent")
            .Select(u => new { u.Id, u.Username, u.Email, u.Role, u.CreatedAt })
            .ToList();
        return Ok(agents);
    }

    // POST /api/admin/agents — create new agent
    [HttpPost("agents")]
    public async Task<IActionResult> CreateAgent([FromBody] RegisterDto dto)
    {
        if (_context.Users.Any(u => u.Email == dto.Email))
            return Conflict(new { message = "Email already exists." });

        if (_context.Users.Any(u => u.Username == dto.Username))
            return Conflict(new { message = "Username already exists." });

        var agent = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "Agent",
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(agent);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Agent created!", agent.Id, agent.Username, agent.Email });
    }

    // DELETE /api/admin/agents/{id} — delete agent
    [HttpDelete("agents/{id}")]
    public async Task<IActionResult> DeleteAgent(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null) return NotFound();
        if (user.Role == "Admin") return BadRequest(new { message = "Cannot delete Admin!" });

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Agent deleted!" });
    }

    // PUT /api/admin/users/{id}/promote — promote to Admin
    [HttpPut("users/{id}/promote")]
    public async Task<IActionResult> Promote(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null) return NotFound();

        user.Role = "Admin";
        await _context.SaveChangesAsync();
        return Ok(new { message = $"{user.Username} promoted to Admin!" });
    }

    // GET /api/admin/users — get all users (customers + agents)
    [HttpGet("users")]
    public IActionResult GetAllUsers()
    {
        var users = _context.Users
            .Select(u => new { u.Id, u.Username, u.Email, u.Role, u.CreatedAt })
            .ToList();
        return Ok(users);
    }
}
