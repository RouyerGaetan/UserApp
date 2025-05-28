using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Authorize]
public class DashboardController : Controller
{
    private readonly IDashboardSectionService _dashboardSectionService;

    public DashboardController(IDashboardSectionService dashboardSectionService)
    {
        _dashboardSectionService = dashboardSectionService;
    }

    // Page principale du dashboard
    public IActionResult Index(string? section)
    {
        if (!string.IsNullOrEmpty(section))
        {
            ViewData["ActiveSection"] = section;
        }
        return View("~/Views/Home/Dashboard.cshtml");
    }

    // Chargement dynamique des sections délégué au service
    public async Task<IActionResult> LoadSection(string section)
    {
        return await _dashboardSectionService.GetSectionAsync(section, User);
    }
}
