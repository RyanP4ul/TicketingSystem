using System.ComponentModel.DataAnnotations;

namespace LanBasedHelpDeskTickingSystem.Entities.DTOs;

public class RemoveAttachmentDto
{
    [Required]
    public int TicketId { get; set; }
    
    [Required]
    public int AttachmentId { get; set; }
}