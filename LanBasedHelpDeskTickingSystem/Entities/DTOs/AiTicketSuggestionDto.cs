using System.ComponentModel.DataAnnotations;

namespace LanBasedHelpDeskTickingSystem.Entities.DTOs;

public class AiTicketSuggestionDto
{
    [Required]
    public string title { get; set; }
    
    [Required]
    public string description { get; set; }
}