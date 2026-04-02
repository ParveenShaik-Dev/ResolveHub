namespace SupportTicket.API.Data;
using Microsoft.EntityFrameworkCore;
using SupportTicket.API.Models;
using SupportTicket.API.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.HasIndex(u => u.Username).IsUnique();
            e.Property(u => u.Role).HasDefaultValue("User");
        });

        // Ticket → CreatedBy (restrict delete to avoid cascade conflict)
        modelBuilder.Entity<Ticket>(e =>
        {
            e.HasOne(t => t.CreatedBy)
             .WithMany(u => u.CreatedTickets)
             .HasForeignKey(t => t.CreatedByUserId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(t => t.AssignedTo)
             .WithMany(u => u.AssignedTickets)
             .HasForeignKey(t => t.AssignedToUserId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // Comment → Ticket / User
        modelBuilder.Entity<Comment>(e =>
        {
            e.HasOne(c => c.Ticket)
             .WithMany(t => t.Comments)
             .HasForeignKey(c => c.TicketId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(c => c.User)
             .WithMany(u => u.Comments)
             .HasForeignKey(c => c.UserId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}