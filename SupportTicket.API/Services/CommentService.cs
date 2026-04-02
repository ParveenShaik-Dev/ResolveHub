using global::SupportTicket.API.DTOs;
using global::SupportTicket.API.Models;
using global::SupportTicket.API.Repositories.Interfaces;
using global::SupportTicket.API.Services.Interfaces;


namespace SupportTicket.API.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _comments;
    private readonly ITicketRepository _tickets;

    public CommentService(ICommentRepository comments, ITicketRepository tickets)
    {
        _comments = comments;
        _tickets = tickets;
    }

    public async Task<CommentDto?> AddAsync(int ticketId, string content, int authorId)
    {
        // Guard: ticket must exist before we add a comment
        var ticket = await _tickets.GetByIdAsync(ticketId);
        if (ticket is null) return null;

        var comment = new Comment
        {
            TicketId = ticketId,
            UserId = authorId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        await _comments.AddAsync(comment);
        await _comments.SaveChangesAsync();

        // Re-fetch so Author navigation property is loaded
        var saved = await _comments.GetByIdAsync(comment.Id);
        if (saved is null) return null;

        return new CommentDto
        {
            Id = saved.Id,
            Content = saved.Content,
            AuthorUsername = saved.User?.Username ?? "Unknown",
            CreatedAt = saved.CreatedAt
        };
    }

    public async Task<bool?> DeleteAsync(int commentId, int requestingUserId, string role)
    {
        var comment = await _comments.GetByIdAsync(commentId);
        if (comment is null) return false;

        // Only the author or an Admin may delete
        if (comment.UserId != requestingUserId && role != "Admin")
            return false;

        await _comments.DeleteAsync(comment);
        await _comments.SaveChangesAsync();
        return true;
    }
}
