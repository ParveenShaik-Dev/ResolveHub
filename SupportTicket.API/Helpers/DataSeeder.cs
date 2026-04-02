using global::SupportTicket.API.Data;
using global::SupportTicket.API.Models;

namespace SupportTicket.API.Helpers;

public static class DataSeeder
{
    public static void SeedAdminUser(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider
            .GetRequiredService<AppDbContext>(); // ← your DbContext name

        if (!context.Users.Any(u => u.Role == "Admin"))
        {
            context.Users.Add(new User
            {
                Username = "admin",
                Email = "admin@company.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            });
            context.SaveChanges();
            Console.WriteLine("Admin user seeded!");
        }
    }
}