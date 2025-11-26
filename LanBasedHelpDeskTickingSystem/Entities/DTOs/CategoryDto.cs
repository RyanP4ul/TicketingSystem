using System.ComponentModel.DataAnnotations;

namespace LanBasedHelpDeskTickingSystem.Entities.DTOs;

public class CategoryDto
{
    [Required]
    public int id { get; set; }
    
    [Required]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
    [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
    public string Name { get; set; }
    
    [Required]
    public string Description { get; set; }
}