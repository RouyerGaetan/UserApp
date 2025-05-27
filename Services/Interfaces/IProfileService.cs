using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using UserApp.Models;
using UserApp.ViewModels;

public interface IProfileService
{
    EditProfileViewModel MapUserToEditProfileViewModel(User user);
    Task<IdentityResult> UpdateUserProfileAsync(User user, EditProfileViewModel model);
}
