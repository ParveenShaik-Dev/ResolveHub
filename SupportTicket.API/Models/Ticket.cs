namespace SupportTicket.API.Models;


public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Open";     // Open | InProgress | Resolved | Closed
    public string Priority { get; set; } = "Medium"; // Low | Medium | High | Critical

    public string UserId { get; set; } = string.Empty;

    public int CreatedByUserId { get; set; }
    public int? AssignedToUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public User CreatedBy { get; set; } = null!;
    public User? AssignedTo { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
