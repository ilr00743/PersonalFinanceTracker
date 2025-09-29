using System.ComponentModel.DataAnnotations;
using PersonalFinanceTracker.Models.Entities;

namespace PersonalFinanceTracker.Models.DTO;

public class CreateCategoryDto
{
    [MaxLength(50)]
    public required string Name { get; set; }
    
    [MaxLength(200)]
    public string? Description { get; set; }
    public CategoryType Type { get; set; }
}