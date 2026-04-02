using global::SupportTicket.API.DTOs;
using global::SupportTicket.API.Helpers;
using global::SupportTicket.API.Models;
using global::SupportTicket.API.Repositories.Interfaces;
using global::SupportTicket.API.Services.Interfaces;


namespace SupportTicket.API.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository users, IConfiguration config)
    {
        _users = users;
        _config = config;
    }

    public async Task<string?> RegisterAsync(RegisterDto dto)
    {
        // Reject duplicates
        if (await _users.EmailExistsAsync(dto.Email))
            return null;
        if (await _users.UsernameExistsAsync(dto.Username))
            return null;

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "Customer",          // default role
            CreatedAt = DateTime.UtcNow
        };

        await _users.AddAsync(user);
        await _users.SaveChangesAsync();

        return JwtHelper.GenerateToken(user, _config);
    }

    public async Task<string?> LoginAsync(LoginDto dto)
    {
        var user = await _users.GetByEmailAsync(dto.Email);
        if (user is null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        return JwtHelper.GenerateToken(user, _config);
    }
}