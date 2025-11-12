using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanBasedHelpDeskTickingSystem.Entities.Models;

public class Category
{
    [Key] 
    [Required, Column("id")] 
    public int Id { get; set; }

    [Required, MaxLength(50)]
    [Column("name")]
    public string Name { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}