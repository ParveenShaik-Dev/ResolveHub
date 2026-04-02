using System.Security.Claims;
using global::SupportTicket.API.DTOs;
using global::SupportTicket.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportTicket.API.Data;

namespace SupportTicket.API.Controllers;

[ApiController]
[Route("api/tickets")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly AppDbContext _context;  // ← change to your DbContext name

    public TicketsController(ITicketService ticketService, AppDbContext context)
    {
        _ticketService = ticketService;
        _context = context;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private string GetUserRole() =>
        User.FindFirstValue(ClaimTypes.Role)!;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        var role = GetUserRole();

        if (role == "Admin" || role == "Agent")
        {
            var all = await _ticketService.GetAllAsync();
            return Ok(all);
        }

        var tickets = await _ticketService.GetByUserIdAsync(userId);
        return Ok(tickets);
    }

    [HttpGet("my-assigned")]
    [Authorize(Roles = "Agent")]
    public async Task<IActionResult> GetMyAssigned()
    {
        var userId = GetUserId();
        var tickets = await _ticketService.GetAssignedAsync(userId);
        return Ok(tickets);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ticket = await _ticketService.GetByIdAsync(id);
        if (ticket is null)
            return NotFound(new { message = $"Ticket {id} not found." });
        return Ok(ticket);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTicketDto dto)
    {
        var userId = GetUserId();
        var ticket = await _ticketService.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateTicketDto dto)
    {
        var userId = GetUserId();
        var userRole = GetUserRole();
        var result = await _ticketService.UpdateStatusAsync(id, dto.Priority, userId, userRole);

        return result switch
        {
            null => NotFound(new { message = $"Ticket {id} not found." }),
            false => Forbid(),
            _ => Ok(result.Value)
        };
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        var userRole = GetUserRole();
        var result = await _ticketService.DeleteAsync(id, userId, userRole);

        return result switch
        {
            null => NotFound(new { message = $"Ticket {id} not found." }),
            false => Forbid(),
            true => NoContent()
        };
    }

    // ── PUT /api/tickets/{id}/assign ──────────────────────────────────────────
    [HttpPut("{id:int}/assign")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Assign(int id, [FromBody] AssignDto dto)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket is null)
            return NotFound(new { message = $"Ticket {id} not found." });

        ticket.AssignedToUserId = dto.AgentId;
        await _context.SaveChangesAsync();
        return Ok(new { message = "Ticket assigned!" });
    }
}