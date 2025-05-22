using System.ComponentModel.DataAnnotations;
using UsersApp.ViewModels;

namespace UsersApp.Attributes
{
    public class RequiredIfOrganisateurAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var model = (RegisterViewModel)validationContext.ObjectInstance;

            if (model.Role == "Organisateur" && string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success!;
        }
    }
}
