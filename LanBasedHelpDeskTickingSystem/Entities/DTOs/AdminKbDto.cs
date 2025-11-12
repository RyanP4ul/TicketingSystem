using System.ComponentModel.DataAnnotations;

namespace LanBasedHelpDeskTickingSystem.Entities.DTOs;

public class AdminKbDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 50 characters")]
    public string Title { get; set; }

    public int AuthorId { get; set; }

    [Required(ErrorMessage = "Content is required.")]
    [MinLength(1, ErrorMessage = "Content cannot be empty")]
    public string Content { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category")]
    public int CategoryId { get; set; } = -1;

    [Required(ErrorMessage = "Tags is required.")]
    [MinLength(1, ErrorMessage = "Tags cannot be empty")]
    public string Tags { get; set; }
}
