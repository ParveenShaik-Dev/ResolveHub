
using SupportTicket.API.Models;


namespace SupportTicket.API.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        Task<Ticket?> GetByIdAsync(int id);
        Task<Ticket?> GetByIdWithDetailsAsync(int id);   // includes Comments + Users
        Task<IEnumerable<Ticket>> GetAllWithDetailsAsync();
        Task<IEnumerable<Ticket>> GetByUserIdAsync(int userId);
        Task AddAsync(Ticket ticket);
        Task UpdateAsync(Ticket ticket);
        Task DeleteAsync(Ticket ticket);
        Task SaveChangesAsync();
        Task<IEnumerable<Ticket>> GetAssignedAsync(int agentId);
    }
}
