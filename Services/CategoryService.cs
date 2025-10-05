using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models.DTO;
using PersonalFinanceTracker.Models.Entities;

namespace PersonalFinanceTracker.Services;

public class CategoryService
{
    private readonly FinanceDbContext _dbContext;
    private readonly IUserService _userService;
    private readonly ILogger<CategoryService> _logger;
    
    public CategoryService(FinanceDbContext dbContext, IUserService userService, ILogger<CategoryService> logger)
    {
        _dbContext = dbContext;
        _userService = userService;
        _logger = logger;
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
        _logger.LogInformation("Creating category {name} for user {userId}", dto.Name, _userService.GetCurrentUserId());
        
        var newCategory = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            Type = dto.Type,
            UserId = _userService.GetCurrentUserId()
        };
        
        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Category {name} created for user {id}", newCategory.Name, newCategory.UserId);
    }

    public async Task UpdateCategory(int id, UpdateCategoryDto dto)
    {
        _logger.LogInformation("Updating category {id} for user {userId}", id, _userService.GetCurrentUserId());
        
        var categoryToUpdate = await GetCategory(id);
        
        if(categoryToUpdate is null)
        {
            _logger.LogWarning("Category {id} not found for user {userId}", id, _userService.GetCurrentUserId());
            return;
        }
        
        categoryToUpdate.Name = dto.Name;
        categoryToUpdate.Description = dto.Description;
        categoryToUpdate.Type = dto.Type;
        
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Category {id} updated for user {userId}", id, _userService.GetCurrentUserId());
    }
    
    public async Task DeleteCategory(int id)
    {
        _logger.LogInformation("Deleting category {id} for user {userId}", id, _userService.GetCurrentUserId());
        
        var categoryToDelete = await GetCategory(id);
        
        if (categoryToDelete is not null)
        {
            _dbContext.Categories.Remove(categoryToDelete);
            await _dbContext.SaveChangesAsync();
            
            _logger.LogInformation("Category {id} deleted for user {userId}", id, _userService.GetCurrentUserId());
        }
        else
        {
            _logger.LogWarning("Category {id} not found for user {userId}", id, _userService.GetCurrentUserId());
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