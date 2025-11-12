using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanBasedHelpDeskTickingSystem.Entities.Models;

public class Ticket
{
    [Key]
    [Required]
    [Column("id")]
    public required int Id { get; set; }
    
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
    [Column("status")]
    public required string Status { get; set; }
    
    [Required]
    [Column("priority")]
    public required string Priority { get; set; }
    
    [Required]
    [Column("category_id")]
    public required int CategoryId { get; set; }
    
    [Required]
    [Column("requester_id")]
    public required int RequesterId { get; set; }
    
    [Required]
    [Column("assigned_to")]
    public required int AssignedId { get; set; }
    
    [Required]
    [Column("created_by")]
    public required int CreatedBy { get; set; }
    
    [Required]
    [Column("resolution")]
    public required string Resolution { get; set; }
    
    [Required]
    [Column("rejection_reason")]
    public required string RejectionReason { get; set; }
    
    [Required]
    [Column("created_at")]
    public required DateTime CreatedAt { get; set; }
    
    [Required]
    [Column("updated_at")]
    public required DateTime UpdatedAt { get; set; }
    
    [Required]
    [Column("resolved_at")]
    public required DateTime ResolvedAt { get; set; }
    
    public User? Requester { get; set; }
    public User? Assigned { get; set; }
    public Category? Category { get; set; }
}