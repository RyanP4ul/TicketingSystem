using System.ComponentModel.DataAnnotations;

namespace LanBasedHelpDeskTickingSystem.Entities.DTOs;

public class CreateTicketDto
{
    [Required]
    [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only letters and numbers are allowed.")]
    public string Title { get; set;}
    
    [Required]
    public string Description { get; set;}
    
    [Required]
    public int CategoryId { get; set; }
    
    [Required]
    public string Room { get; set; }
    
    public List<IFormFile> Files { get; set; } = new();
}