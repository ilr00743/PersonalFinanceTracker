
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Models.Entities;

namespace PersonalFinanceTracker.Data;

public class FinanceDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Budget> Budgets { get; set; } = null!;

    public FinanceDbContext(DbContextOptions options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Transactions)
            .WithOne()
            .HasForeignKey(t => t.UserId);
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.Categories)
            .WithOne()
            .HasForeignKey(c => c.UserId);

        modelBuilder.Entity<Budget>()
            .HasOne(b => b.Category)
            .WithMany()
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Budget>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}