using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanBasedHelpDeskTickingSystem.Entities.Models;

public class Notification
{
        [Key]
        [Required]
        [Column("id")]
        public required int Id { get; set; }
    
        [Required, MaxLength(200)]
        [Column("title")]
        public required string Title { get; set; }
        
        [Required]
        [Column("message")]
        public required string Message { get; set; }
        
        [Required]
        [Column("type")]
        public required string Type { get; set; }
        
        [Required]
        [Column("is_read")]
        public required bool IsRead { get; set; }
        
        [Required]
        [Column("created_at")]
        public required DateTime CreatedAt { get; set; }
}