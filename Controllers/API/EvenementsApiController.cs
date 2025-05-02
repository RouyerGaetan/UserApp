using Microsoft.AspNetCore.Mvc;
using UserApp.Data;
using UserApp.Models;
using Microsoft.EntityFrameworkCore;

namespace UserApp.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class EvenementsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EvenementsApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvenements()
        {
            var evenements = await _context.Evenements.ToListAsync();
            return Ok(evenements); // retourne du JSON
        }
    }
}
