using LanBasedHelpDeskTickingSystem.Entities.Enums;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/notifications")]
public class NotificationApiController(INotifyRepository notifyRepository) : ControllerBase
{
    
    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var userId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "");
        var notifications = await notifyRepository.GetLatestNotifiesById(userId);
        
        return Ok(notifications);
    }
    
    [HttpGet("redirect/{id}")]
    public async Task<IActionResult> GetNotificationRedirect(int id)
    {
        var notifications = await notifyRepository.GetNotificationById(id);

        if (notifications == null)
        {
            return NotFound();
        }
        
        await notifyRepository.UpdateReadStatus(id);
        
        var ticketId = notifications.Ticket?.Id;

        return notifications.User?.Roles switch
        {
            UserRole.Admin => Redirect($"/admin/tickets/{ticketId}"),
            UserRole.Technician => Redirect($"/technician/tickets/{ticketId}"),
            _ => Redirect($"/user/tickets/{ticketId}")
        };
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteNotification()
    {
        var userId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "");
        await notifyRepository.ClearNotificationsByUserId(userId);
        return NoContent();
    }
    
}