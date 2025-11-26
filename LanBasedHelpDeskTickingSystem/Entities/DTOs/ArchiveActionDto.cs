
using System.ComponentModel.DataAnnotations;

namespace LanBasedHelpDeskTickingSystem.Entities.DTOs;

public class ArchiveActionDto
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    public string Type { get; set; }
    
    [Required]
    public string Action { get; set; }
}