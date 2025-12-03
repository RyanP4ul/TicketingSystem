using System.ComponentModel.DataAnnotations;

namespace LanBasedHelpDeskTickingSystem.Entities.DTOs;

public class CreateCommentDto
{
    [Required]
    public int ticketId { get; set; }
    
    [Required]
    public string content { get; set; }
}