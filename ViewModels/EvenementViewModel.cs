using UserApp.Models;

public class EvenementViewModel
{
    public Evenement Evenement { get; set; }
    public IEnumerable<string> Sports { get; set; } = new List<string>();
}
