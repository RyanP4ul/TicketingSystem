using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanBasedHelpDeskTickingSystem.Entities.Models;

public class Notification
{
        [Key]
        [Required]
        [Column("id")]
        public int Id { get; set; }
        
        [Required]
        [Column("ticket_id")]
        public int TicketId { get; set; }
    
        [Required]
        [Column("user_id")]
        public int UserId { get; set; }
        
        [Required]
        [Column("message")]
        public string message { get; set; }
        
        [Required]
        [Column("is_read")]
        public bool IsRead { get; set; }
        
        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        public Ticket? Ticket;
        public User? User;

}