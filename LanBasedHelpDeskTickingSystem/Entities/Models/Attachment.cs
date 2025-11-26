using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanBasedHelpDeskTickingSystem.Entities.Models;

[Table("tickets_attachements")]
public class Attachment
{
    [Key]
    [Required]
    [Column("id")]
    public int id { get; set; }
    
    [Required]
    [Column("ticket_id")]
    public int TicketId { get; set; }
    
    [Required]
    [Column("file_name")]
    public string FileName { get; set; }
    
    [Required]
    [Column("file_extension")]
    public string FileExtension { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    public Ticket? Ticket { get; set; }
    
}