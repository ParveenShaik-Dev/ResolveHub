namespace SupportTicket.API.Services.Interfaces;
using global::SupportTicket.API.DTOs;

public interface ICommentService
{
    Task<CommentDto?> AddAsync(int ticketId, string content, int authorId);
    Task<bool?> DeleteAsync(int commentId, int requestingUserId, string role);
}