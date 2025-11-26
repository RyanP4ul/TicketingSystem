using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanBasedHelpDeskTickingSystem.Entities.Models;

[Table("categories")]
public class Category
{
    [Key] 
    [Required, Column("id")] 
    public int Id { get; set; }

    [Required, MaxLength(50)]
    [Column("name")]
    public string Name { get; set; }
    
    [Required]
    [Column("description")]
    public string Description { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}