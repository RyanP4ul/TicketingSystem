using System.ComponentModel.DataAnnotations;

namespace LanBasedHelpDeskTickingSystem.Entities.DTOs;

public class ViewTicketDto
{

    [Required]
    public string Title { get; set; }
    
    [Required]
    public string Description { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
    
    [Required]
    public string Priority { get; set; }
    
    [Required]
    public string Room { get; set; }

    public List<IFormFile> Files { get; set; } = new();
}