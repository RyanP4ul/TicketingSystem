using System.ComponentModel.DataAnnotations;

namespace LanBasedHelpDeskTickingSystem.Entities.DTOs;

public class ViewTicketDto
{

    [Required]
    public string Description { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
    
    [Required]
    public string Priority { get; set; }
}