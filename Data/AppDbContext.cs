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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ajouter des sports par défaut
            modelBuilder.Entity<Sport>().HasData(
                new Sport { Id = 1, Name = "Football" },
                new Sport { Id = 2, Name = "Basketball" },
                new Sport { Id = 3, Name = "Tennis" },
                new Sport { Id = 4, Name = "Rugby" },
                new Sport { Id = 5, Name = "Handball" }
            );
        }
    }
}

