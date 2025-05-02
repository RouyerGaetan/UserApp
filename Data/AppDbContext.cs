using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserApp.Models;

namespace UserApp.Data
{
    public class AppDbContext : IdentityDbContext<Users>
    {
        public DbSet<Evenement> Evenements { get; set; }
        public DbSet<Sport> Sports { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}

