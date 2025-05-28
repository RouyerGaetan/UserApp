using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using UserApp.Models;
using UserApp.Services;

[Authorize]
public class HistoriqueController : Controller
{
    private readonly IReservationService _reservationService;
    private readonly UserManager<User> _userManager;

    public HistoriqueController(IReservationService reservationService, UserManager<User> userManager)
    {
        _reservationService = reservationService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetHistoriquePartial()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var historique = await _reservationService.GetPastReservationsAsync(user.Id);

        return PartialView("~/Views/Home/Partials/Shared/_Historique.cshtml", historique);
    }
}
