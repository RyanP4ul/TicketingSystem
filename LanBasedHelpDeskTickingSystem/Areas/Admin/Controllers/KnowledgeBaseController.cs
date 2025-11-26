using LanBasedHelpDeskTickingSystem.Entities.Enums;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = nameof(UserRole.Admin))]
[Route("/Admin/KnowledgeBase")]
public class KnowledgeBaseController(ICategoryRepository categoryRepository, IKnowledgeBaseRepository knowledgeBaseRepository) : Controller
{
    
    public async Task<IActionResult> Index()
    {
        var categories = await categoryRepository.GetAllCategoriesAsync();
        
        ViewBag.Categories = categories;
        ViewBag.TotalArticles = await knowledgeBaseRepository.GetTotalArticlesAsync();
        ViewBag.TotalDrafs = await knowledgeBaseRepository.GetTotalDraftsArticlesAsync();
        ViewBag.TotalPublishedArticles = await knowledgeBaseRepository.GetTotalPublishedArticlesAsync();
        ViewBag.TotalViewArticles = await knowledgeBaseRepository.GetTotalViewArticleAsync();
        
        return View("Admin/KnowledgeBase/KBLists");
    }
    
}