namespace SupportTicket.API.Services.Interfaces;

public interface IAuthService
{
    Task<string?> RegisterAsync(DTOs.RegisterDto dto);
    Task<string?> LoginAsync(DTOs.LoginDto dto);
}