using Microsoft.AspNetCore.Identity;
using UserApp.Models;

public class Reservation
{
    public int Id { get; set; }
    public DateTime ReservationDate { get; set; }
    public int NumberOfSeats { get; set; }
    public string Status { get; set; }
    public bool IsPresent { get; set; }
    public string QRcode { get; set; }

    // Liens avec d'autres entités
    public string UserId { get; set; }
    public virtual Users User { get; set; }

    public int EvenementId { get; set; }
    public virtual Evenement Evenement { get; set; }
}



