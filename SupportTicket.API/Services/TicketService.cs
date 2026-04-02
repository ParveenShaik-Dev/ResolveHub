using global::SupportTicket.API.DTOs;
using global::SupportTicket.API.Models;
using global::SupportTicket.API.Repositories.Interfaces;
using global::SupportTicket.API.Services.Interfaces;


namespace SupportTicket.API.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _tickets;

    public TicketService(ITicketRepository tickets)
    {
        _tickets = tickets;
    }

    // ── Create ────────────────────────────────────────────────────────────────

    public async Task<TicketResponseDto?> CreateAsync(CreateTicketDto dto, int userId)
    {
        var ticket = new Ticket
        {
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            Status = "Open",
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _tickets.AddAsync(ticket);
        await _tickets.SaveChangesAsync();

        // Re-fetch so navigation properties are populated for the response
        var created = await _tickets.GetByIdWithDetailsAsync(ticket.Id);
        return created is null ? null : MapToDto(created);
    }

    // ── Read ──────────────────────────────────────────────────────────────────

    public async Task<TicketResponseDto?> GetByIdAsync(int id)
    {
        var ticket = await _tickets.GetByIdWithDetailsAsync(id);
        return ticket is null ? null : MapToDto(ticket);
    }

    public async Task<IEnumerable<TicketResponseDto>> GetAllAsync()
    {
        var tickets = await _tickets.GetAllWithDetailsAsync();
        return tickets.Select(MapToDto);
    }

    public async Task<IEnumerable<TicketResponseDto>> GetByUserIdAsync(int userId)
    {
        var tickets = await _tickets.GetByUserIdAsync(userId);
        return tickets.Select(MapToDto);
    }

    // ── Update ────────────────────────────────────────────────────────────────

    public async Task<bool?> UpdateStatusAsync(
        int id, string status, int requestingUserId, string role)
    {
        var ticket = await _tickets.GetByIdAsync(id);
        if (ticket is null) return false;

        // Only the owner or an Admin may change status
        if (ticket.CreatedByUserId != requestingUserId && role != "Admin")
            return false;

        ticket.Status = status;
        await _tickets.UpdateAsync(ticket);
        await _tickets.SaveChangesAsync();
        return true;
    }

    // ── Delete ────────────────────────────────────────────────────────────────

    public async Task<bool?> DeleteAsync(int id, int requestingUserId, string role)
    {
        var ticket = await _tickets.GetByIdAsync(id);
        if (ticket is null) return false;

        if (ticket.CreatedByUserId != requestingUserId && role != "Admin")
            return false;

        await _tickets.DeleteAsync(ticket);
        await _tickets.SaveChangesAsync();
        return true;
    }

    // ── Mapping ───────────────────────────────────────────────────────────────

    private static TicketResponseDto MapToDto(Ticket t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        Priority = t.Priority,
        Status = t.Status,
        CreatedAt = t.CreatedAt,
        CreatedByUsername = t.CreatedBy?.Username ?? "Unknown",
        Comments = t.Comments.Select(c => new CommentDto
        {
            Id = c.Id,
            Content = c.Content,
            AuthorUsername = c.User?.Username ?? "Unknown",
            CreatedAt = c.CreatedAt
        }).ToList()
    };
    public async Task<IEnumerable<TicketResponseDto>> GetAssignedAsync(int agentId)
    {
        var tickets = await _tickets.GetAssignedAsync(agentId);
        return tickets.Select(MapToDto);
    }
}