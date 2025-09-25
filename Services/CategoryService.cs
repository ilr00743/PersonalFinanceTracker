using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models.Entities;

namespace PersonalFinanceTracker.Services;

public class CategoryService
{
    private readonly FinanceDbContext _dbContext;
    
    public CategoryService(FinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Category>?> GetCategories()
    {
        return await _dbContext.Categories.ToListAsync();
    }
    
    public async Task<Category?> GetCategory(int id)
    {
        return await _dbContext.Categories.FindAsync(id);
    }

    public async Task CreateCategory(Category category)
    {
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateCategory(int id, Category category)
    {
        var categoryToUpdate = await GetCategory(id);
        
        categoryToUpdate.Name = category.Name;
        categoryToUpdate.Description = category.Description;
        categoryToUpdate.Type = category.Type;
        
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task DeleteCategory(int id)
    {
        var categoryToDelete = await GetCategory(id);
        
        if (categoryToDelete is not null)
        {
            _dbContext.Categories.Remove(categoryToDelete);
            await _dbContext.SaveChangesAsync();
        } 
    }
    
    public async Task<bool> IsCategoryAlreadyExist(string name, int? excludeId = null)
    {
        var query = _dbContext.Categories.AsQueryable();
        if (excludeId is not null)
            query = query.Where(c => c.Id != excludeId);

        return await query.AnyAsync(c => c.Name == name);
    }
    
    public async Task<bool> IsCategoryAlreadyExist(int id)
    {
        return await _dbContext.Categories.AnyAsync(c => c.Id == id);
    }
}