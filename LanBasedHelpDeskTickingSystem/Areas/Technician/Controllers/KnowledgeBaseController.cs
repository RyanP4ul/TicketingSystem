using LanBasedHelpDeskTickingSystem.Entities.Enums;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Areas.Technician.Controllers;

[Area("Technician")]
[Authorize(Roles = nameof(UserRole.Technician))]
[Route("/Technician/KnowledgeBase")]
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
        
        return View("Technician/KnowledgeBase/KBLists");
    }
    
}