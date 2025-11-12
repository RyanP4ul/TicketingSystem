using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanBasedHelpDeskTickingSystem.Entities.Models;

public class KnowledgeBase
{
    [Key]
    [Required]
    [Column("id")]
    public int Id { get; set; }
    
    [Required, MaxLength(200)]
    [Column("title")]
    public required string Title { get; set; }
    
    [Required]
    [Column("content")]
    public required string Content { get; set; }
    
    [Required]
    [Column("category_id")]
    public required int CategoryId { get; set; }
    
    [Required]
    [Column("author_id")]
    public required int AuthorId { get; set; }
    
    [Column("is_published")]
    public bool IsPublished { get; set; }
    
    [Required]
    [Column("tags")]
    public required string Tags { get; set; }
    
    [Column("view_count")]
    public int ViewCount { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    public Category? Category { get; set; }
    public User? Author { get; set; }
}