using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Models.Entities;
using PersonalFinanceTracker.Services;

namespace PersonalFinanceTracker.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoriesController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _categoryService.GetCategories();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory(int id)
    {
        var category = await _categoryService.GetCategory(id);
        
        if (category is null)
        {
            return NotFound("Category not found");
        }
        
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> CreateCategory(Category category)
    {
        if (await _categoryService.IsCategoryExist(category.Name) || await _categoryService.IsCategoryExist(category.Id))
        {
            return Conflict("Category already exists");
        }
        
        await _categoryService.CreateCategory(category);
        return CreatedAtAction(nameof(GetCategory), new {id = category.Id}, category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, Category category)
    {
        if (id != category.Id)
        {
            return BadRequest();
        }

        if (await _categoryService.IsCategoryExist(category.Name, excludeId: id))
        {
            return Conflict("Category already exists");
        }

        if (!await _categoryService.IsCategoryExist(id))
        {
            return NotFound("Category not found");
        }
        
        await _categoryService.UpdateCategory(id, category);

        return Ok(category);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        if (!await _categoryService.IsCategoryExist(id))
        {
            return NotFound("Category not found");
        }

        if (await _categoryService.HasTransactions(id))
        {
            return Conflict("Cannot delete the category because it is already in the transactions.");
        }
        
        await _categoryService.DeleteCategory(id);
        return NoContent();
    }
}