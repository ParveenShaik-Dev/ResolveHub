using System.Security.Claims;
using global::SupportTicket.API.DTOs;
using global::SupportTicket.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SupportTicket.API.Controllers;

[ApiController]
[Route("api/tickets/{ticketId:int}/comments")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    // ── helper ─────────────────────────────────────────────────────────────────

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private string GetUserRole() =>
        User.FindFirstValue(ClaimTypes.Role)!;

    // ── POST /api/tickets/{ticketId}/comments ──────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> AddComment(int ticketId, [FromBody] CommentDto dto)
    {
        var userId = GetUserId();
        var comment = await _commentService.AddAsync(ticketId, dto.Content, userId);

        if (comment is null)
            return NotFound(new { message = $"Ticket {ticketId} not found." });

        return CreatedAtAction(nameof(AddComment),
            new { ticketId, commentId = comment.Id },
            comment);
    }

    // ── DELETE /api/tickets/{ticketId}/comments/{commentId} ───────────────────

    [HttpDelete("{commentId:int}")]
    public async Task<IActionResult> DeleteComment(int ticketId, int commentId)
    {
        var userId = GetUserId();
        var userRole = GetUserRole();

        var result = await _commentService.DeleteAsync(
             commentId, userId, userRole);

        return result switch
        {
            null => NotFound(new { message = $"Comment {commentId} not found on ticket {ticketId}." }),
            false => Forbid(),
            true => NoContent()
        };
    }
}