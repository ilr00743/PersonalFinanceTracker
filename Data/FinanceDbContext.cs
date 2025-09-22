
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Models.Entities;

namespace PersonalFinanceTracker.Data;

public class FinanceDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; } = null!;
    
    public FinanceDbContext(DbContextOptions options) : base(options)
    {
        
    }
}