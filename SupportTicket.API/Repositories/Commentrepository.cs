
using Microsoft.EntityFrameworkCore;
using SupportTicket.API.Data;
using SupportTicket.API.Models;
using SupportTicket.API.Repositories.Interfaces;


namespace SupportTicket.API.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;

        public CommentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Comment?> GetByIdAsync(int id)
            => await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IEnumerable<Comment>> GetByTicketIdAsync(int ticketId)
            => await _context.Comments
                .Include(c => c.User)
                .Where(c => c.TicketId == ticketId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

        public async Task AddAsync(Comment comment)
            => await _context.Comments.AddAsync(comment);

        public Task DeleteAsync(Comment comment)
        {
            _context.Comments.Remove(comment);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}