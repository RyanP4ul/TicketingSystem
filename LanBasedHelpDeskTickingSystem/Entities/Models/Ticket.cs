using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanBasedHelpDeskTickingSystem.Entities.Models;

public class Ticket
{
    [Key]
    [Required]
    [Column("id")]
    public int Id { get; set; }
    
    [Required, MaxLength(20)]
    [Column("ticket_number")]
    public required string TicketNumber { get; set; }
    
    [Required, MaxLength(50)]
    [Column("title")]
    public required string Title { get; set; }
    
    [Required]
    [Column("description")]
    public required string Description { get; set; }
    
    [Required]
    [Column("room")]
    public required string Room { get; set; }
    
    [Required]
    [Column("status")]
    public string Status { get; set; }
    
    [Column("priority")]
    public string Priority { get; set; }
    
    [Required]
    [Column("category_id")]
    public required int CategoryId { get; set; }
    
    [Required]
    [Column("requester_id")]
    public int RequesterId { get; set; }

    [Column("assigned_to")] 
    public int? AssignedId { get; set; }
    
    [Column("resolution")]
    public string? Resolution { get; set; }
    
    [Column("rejection_reason")]
    public string? RejectionReason { get; set; }

    [Column("is_deleted")] 
    public bool IsDeleted { get; set; } = false;
    
    [Column("deleted_by")]
    public int? DeletedBy { get; set; }
    
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    [Required]
    [Column("resolved_at")]
    public DateTime ResolvedAt { get; set; }
    
    [Required]
    [Column("deleted_at")]
    public DateTime DeletedAt { get; set; }
    
    [Required]
    [Column("view_count")]
    public int ViewCount { get; set; }
    
    public User? Requester { get; set; }
    public User? Assigned { get; set; }
    public Category? Category { get; set; }
    public ICollection<Attachment> Attachments { get; set; }
    
}