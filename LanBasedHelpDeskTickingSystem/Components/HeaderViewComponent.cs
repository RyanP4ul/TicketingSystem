using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Components;

public class HeaderViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View(); // renders Views/Shared/Components/NavMenu/Default.cshtml
    }
}