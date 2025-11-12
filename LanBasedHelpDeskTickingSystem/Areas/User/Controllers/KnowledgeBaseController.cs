using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Enums;
using LanBasedHelpDeskTickingSystem.Entities.Views;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Areas.User.Controllers;

[Area("User")]
[Route("User/KnowledgeBase")]
[Authorize(Roles = nameof(UserRole.User))]
public class KnowledgeBaseController(AppDbContext db, ICategoryRepository categoryRepository, IKnowledgeBaseRepository knowledgeBaseRepository) : Controller
{
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var categories = await categoryRepository.GetAllCategoriesAsync();
        
        return View("Student/KnowledgeBase/Index", new UserKbViewModel
        {
            Categories = categories
        });
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> ViewArticlesByCategory(int id)
    {
        var article = await knowledgeBaseRepository.GetArticleByIdAsync(id);
        
        if (article == null)
        {
            return RedirectToAction("Index");
        }
        
        return View("Student/KnowledgeBase/ViewKb", article);
    }
}