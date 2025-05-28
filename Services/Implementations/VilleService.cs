public class VilleService : IVilleService
{
    public List<String> GetAllVilles()
    {
        return new List<string>
        {
            "Paris", "Marseille", "Lyon", "Toulouse", "Nice",
            "Nantes", "Montpellier", "Strasbourg", "Bordeaux", "Lille",
            "Rennes", "Reims", "Saint-Étienne", "Le Havre", "Grenoble",
            "Dijon", "Angers", "Nîmes", "Clermont-Ferrand", "Metz"
        };
    }
}
