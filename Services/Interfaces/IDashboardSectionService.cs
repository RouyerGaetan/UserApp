using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public interface IDashboardSectionService
{
    Task<IActionResult> GetSectionAsync(string section, ClaimsPrincipal user);
}
