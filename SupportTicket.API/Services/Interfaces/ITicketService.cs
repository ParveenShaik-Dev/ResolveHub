namespace SupportTicket.API.Services.Interfaces;
using global::SupportTicket.API.DTOs;

public interface ITicketService
{
    Task<TicketResponseDto?> CreateAsync(CreateTicketDto dto, int userId);
    Task<TicketResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<TicketResponseDto>> GetAllAsync();
    Task<IEnumerable<TicketResponseDto>> GetByUserIdAsync(int userId);
    Task<bool?> UpdateStatusAsync(int id, string status, int requestingUserId, string role);
    Task<bool?> DeleteAsync(int id, int requestingUserId, string role);
    Task<IEnumerable<TicketResponseDto>> GetAssignedAsync(int agentId);
}