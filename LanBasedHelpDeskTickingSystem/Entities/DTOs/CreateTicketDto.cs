using System.ComponentModel.DataAnnotations;

namespace LanBasedHelpDeskTickingSystem.Entities.DTOs;

public class CreateTicketDto
{
    [Required]
    public int RequesterId { get; set; }
    
    [Required]
    public string Title { get; set;}
    
    [Required]
    public string Description { get; set;}
    
    [Required]
    public int CategoryId { get; set; }
    
    [Required]
    public string Priority { get; set; }
}