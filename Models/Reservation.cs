using Microsoft.AspNetCore.Identity;
using UserApp.Models;

public class Reservation
{
    public int Id { get; set; }

    public int EvenementId { get; set; }
    public Evenement Evenement { get; set; }

    public string UserId { get; set; }
    public IdentityUser User { get; set; }

    public int NombrePlaces { get; set; }

    public DateTime DateReservation { get; set; } = DateTime.Now;
}

