using LanBasedHelpDeskTickingSystem.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Data;

public class AppDbContext : DbContext
{
    
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

    public DbSet<T> SetEntity<T>() where T : class => Set<T>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().ToTable("categories");

        #region KnowledgeBase
        modelBuilder.Entity<KnowledgeBase>().ToTable("knowledge_base");
        
        modelBuilder.Entity<KnowledgeBase>()
            .HasOne(kb => kb.Category)
            .WithMany()
            .HasForeignKey(kb => kb.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<KnowledgeBase>()
            .HasOne(kb => kb.Author)
            .WithMany()
            .HasForeignKey(kb => kb.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
        #endregion
        
        modelBuilder.Entity<User>()
            .ToTable("users")
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Ticket>().ToTable("tickets");

        // Requester → Tickets
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Requester)
            .WithMany()
            .HasForeignKey(t => t.RequesterId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Assigned Technician → Tickets
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Assigned)
            .WithMany()
            .HasForeignKey(t => t.AssignedId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Category → Tickets
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Category)
            .WithMany()
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<User>()
            .ToTable("users")
            .HasIndex(u => u.Email)
            .IsUnique();
        
        modelBuilder.Entity<User>()
            .Property(u => u.Roles)
            .HasConversion<string>()
            .HasMaxLength(50);
    }
}