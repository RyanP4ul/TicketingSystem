using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanBasedHelpDeskTickingSystem.Entities.Models;

[Table("comments")]
public class Comment
{
    
    [Key] 
    [Required, Column("id")] 
    public int Id { get; set; }
    
    [Required, Column("ticket_id")] 
    public int TicketId { get; set; }
    
    [Required, Column("user_id")]
    public int UserId { get; set; }
    
    [Required]
    [Column("content")]
    public string Content { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Ticket? Ticket { get; set; }
    public User? User { get; set; }
    
}