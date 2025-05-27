using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using UserApp.Data;
using UserApp.Models;
using UserApp.ViewModels;

public class ProfileService : IProfileService
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;

    public ProfileService(UserManager<User> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public EditProfileViewModel MapUserToEditProfileViewModel(User user)
    {
        return new EditProfileViewModel
        {
            Nom = user.Nom,
            Prenom = user.Prenom,
            Birthdate = user.Birthdate,
            AvatarURL = user.AvatarURL,
            NomDuClub = user.Club?.Nom,
            PhoneNumber = user.PhoneNumber
        };
    }

    public async Task<IdentityResult> UpdateUserProfileAsync(User user, EditProfileViewModel model)
    {
        user.Nom = model.Nom;
        user.Prenom = model.Prenom;
        user.Birthdate = model.Birthdate;
        user.AvatarURL = model.AvatarURL;
        user.PhoneNumber = model.PhoneNumber;

        if (user.Club != null)
        {
            user.Club.Nom = model.NomDuClub;
        }

        var result = await _userManager.UpdateAsync(user);
        await _context.SaveChangesAsync();

        return result;
    }
}
