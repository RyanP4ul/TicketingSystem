using System.ComponentModel.DataAnnotations;

namespace LanBasedHelpDeskTickingSystem.Entities.DTOs;

public class AiDto
{
    [Required]
    public string prompt { get; set; }
}