using LanBasedHelpDeskTickingSystem.Entities.DTOs;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/comments")]
public class CommentApiController(ICommentRepository commentRepository) : ControllerBase
{
    
    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                );
        
            return BadRequest(new { success = false, errors });
        }

        var userId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "");
        var response = await commentRepository.CreateCommentAsync(dto.ticketId, userId, dto.content);
        
        if (!response.Success) return BadRequest(response);
        
        return Ok(response);
    }
    
}