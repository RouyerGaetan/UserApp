using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserApp.Models;

namespace UserApp.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Evenement> Evenements { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Athlete> Athletes { get; set; }
        public DbSet<NoteEvenement> NotesEvenements { get; set; }
        public DbSet<NoteAthlete> NotesAthletes { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relation 1-1 User <-> Club
            modelBuilder.Entity<User>()
                .HasOne(u => u.Club)
                .WithOne(c => c.User)
                .HasForeignKey<Club>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Evenement -> Club (1-N)
            modelBuilder.Entity<Evenement>()
                .HasOne(e => e.Club)
                .WithMany(c => c.Evenements)
                .HasForeignKey(e => e.ClubId)
                .OnDelete(DeleteBehavior.Cascade);

            // Athlete -> Club (1-N)
            modelBuilder.Entity<Athlete>()
                .HasOne(a => a.Club)
                .WithMany(c => c.Athletes)
                .HasForeignKey(a => a.ClubId)
                .OnDelete(DeleteBehavior.Cascade);

            // NoteAthlete -> Athlete
            modelBuilder.Entity<NoteAthlete>()
                .HasOne(n => n.Athlete)
                .WithMany(a => a.Notes)
                .HasForeignKey(n => n.AthleteId)
                .OnDelete(DeleteBehavior.Restrict);

            // NoteAthlete -> Evenement
            modelBuilder.Entity<NoteAthlete>()
                .HasOne(n => n.Evenement)
                .WithMany()
                .HasForeignKey(n => n.EvenementId)
                .OnDelete(DeleteBehavior.Restrict); // ✅ déjà OK

            // NoteAthlete -> User
            modelBuilder.Entity<NoteAthlete>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict); // 🔄 Change cascade → restrict

            // NoteEvenement -> Evenement
            modelBuilder.Entity<NoteEvenement>()
                .HasOne(n => n.Evenement)
                .WithMany()
                .HasForeignKey(n => n.EvenementId)
                .OnDelete(DeleteBehavior.Restrict); // 🔄 Change cascade → restrict

            // NoteEvenement -> User
            modelBuilder.Entity<NoteEvenement>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict); // 🔄 Change cascade → restrict

            // Reservation -> User
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict); // 🔄 Change cascade → restrict

            // Reservation -> Evenement
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Evenement)
                .WithMany()
                .HasForeignKey(r => r.EvenementId)
                .OnDelete(DeleteBehavior.Cascade); // ✅ Garde cascade ici
        }
    }
}
