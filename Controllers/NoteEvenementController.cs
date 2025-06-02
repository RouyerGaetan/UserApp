using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using UserApp.Models;
using UserApp.Services.Interfaces;

[Authorize(Roles = "User,Organisateur,Admin")]
public class NoteEvenementController : Controller
{
    private readonly INoteEvenementService _noteService;

    public NoteEvenementController(INoteEvenementService noteService)
    {
        _noteService = noteService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitNote(NoteEvenementViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Le formulaire est invalide. Veuillez vérifier les informations.";
            return RedirectToAction("Index", "Dashboard", new { section = "historique" });
        }

        try
        {
            // Vérifie si l'utilisateur a déjà noté cet événement  
            var dejaNote = await _noteService.HasUserAlreadyNotedAsync(userId, model.EvenementId);
            if (dejaNote)
            {
                TempData["Error"] = "Vous avez déjà voté pour cet événement.";
                return RedirectToAction("Index", "Dashboard", new { section = "historique" });
            }

            // Création de l'entité NoteEvenement à partir du ViewModel + infos utilisateur
            var note = new NoteEvenement
            {
                UserId = userId,
                EvenementId = model.EvenementId,
                Note = model.Note,
                Commentaire = model.Commentaire,
                DateVote = DateTime.Now
            };

            // Soumet la note  
            await _noteService.SubmitNoteAsync(note);

            TempData["Success"] = "Merci pour votre vote !";
        }
        catch
        {
            TempData["Error"] = "Une erreur est survenue lors de l'enregistrement de votre vote.";
        }

        return RedirectToAction("Index", "Dashboard", new { section = "historique" });
    }

    [Authorize(Roles = "Organisateur,Admin")]
    public async Task<IActionResult> GetAvisPartial()
    {
        var organisateurId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(organisateurId))
            return Unauthorized();

        var notesParEvenement = await _noteService.GetNotesParEvenementPourOrganisateurAsync(organisateurId);
        return PartialView("~/Views/Home/Partials/Organisateur/_Avis.cshtml", notesParEvenement);
    }
}
