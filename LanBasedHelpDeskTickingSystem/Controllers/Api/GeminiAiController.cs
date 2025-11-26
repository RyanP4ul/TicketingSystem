using LanBasedHelpDeskTickingSystem.Entities.DTOs;
using LanBasedHelpDeskTickingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Controllers.Api;

[Authorize]
public class GeminiAiController(IAiSupportService aiService) : ControllerBase
{
    
    [HttpPost]
    [Route("api/ai/support")]
    public async Task<IActionResult> GetSupportAnswer([FromBody] AiDto dto)
    {
        if (string.IsNullOrEmpty(dto.prompt))
        {
            return BadRequest(new { error = "Prompt cannot be empty." });
        }
        
        var answer = await aiService.GetAnswerFromArticleAsync(dto.prompt);
        return Ok(new { answer });
    }
    
}