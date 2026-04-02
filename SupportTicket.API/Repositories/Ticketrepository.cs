using Microsoft.EntityFrameworkCore;
using SupportTicket.API.Data;
using SupportTicket.API.Models;
using SupportTicket.API.Repositories.Interfaces;


namespace SupportTicket.API.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _context;

        public TicketRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Ticket?> GetByIdAsync(int id)
            => await _context.Tickets.FindAsync(id);

        public async Task<Ticket?> GetByIdWithDetailsAsync(int id)
            => await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(t => t.Id == id);

        public async Task<IEnumerable<Ticket>> GetAllWithDetailsAsync()
            => await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<Ticket>> GetByUserIdAsync(int userId)
            => await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .Where(t => t.CreatedByUserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

        public async Task AddAsync(Ticket ticket)
            => await _context.Tickets.AddAsync(ticket);

        public Task UpdateAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public async Task<IEnumerable<Ticket>> GetAssignedAsync(int agentId)
        {
            return await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Comments)
                .Where(t => t.AssignedToUserId == agentId)
                .ToListAsync();
        }
    }
}
