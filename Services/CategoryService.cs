using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models.DTO;
using PersonalFinanceTracker.Models.Entities;

namespace PersonalFinanceTracker.Services;

public class CategoryService
{
    private readonly FinanceDbContext _dbContext;
    private readonly IUserService _userService;
    
    public CategoryService(FinanceDbContext dbContext, IUserService userService)
    {
        _dbContext = dbContext;
        _userService = userService;
    }

    public async Task<List<Category>?> GetCategories()
    {
        return await _dbContext.Categories
            .Where(c => c.UserId == _userService.GetCurrentUserId())
            .ToListAsync();
    }
    
    public async Task<Category?> GetCategory(int id)
    {
        return await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == _userService.GetCurrentUserId());
    }

    public async Task CreateCategory(CreateCategoryDto dto)
    {
        var newCategory = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            Type = dto.Type,
            UserId = _userService.GetCurrentUserId()
        };
        
        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateCategory(int id, UpdateCategoryDto dto)
    {
        var categoryToUpdate = await GetCategory(id) ?? throw new NullReferenceException("Category not found");
        
        categoryToUpdate.Name = dto.Name;
        categoryToUpdate.Description = dto.Description;
        categoryToUpdate.Type = dto.Type;
        
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
    
    public async Task<bool> IsCategoryExist(string name, int? excludeId = null)
    {
        var query = _dbContext.Categories.AsQueryable();
        if (excludeId is not null)
            query = query.Where(c => c.Id != excludeId);

        return await query.AnyAsync(c => c.Name == name && c.UserId == _userService.GetCurrentUserId());
    }
    
    public async Task<bool> IsCategoryExist(int id)
    {
        return await _dbContext.Categories.AnyAsync(c => c.Id == id && c.UserId == _userService.GetCurrentUserId());
    }

    public async Task<bool> HasTransactions(int id)
    {
        return await _dbContext.Transactions.AnyAsync(t => t.CategoryId == id && t.UserId == _userService.GetCurrentUserId());
    }
}