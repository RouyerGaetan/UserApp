namespace UserApp.Services
{
    public class SportService : ISportService
    {
        public List<string> GetAllSports()
        {
            return new List<string>
            {
                "Football", 
                "Basketball", 
                "Tennis", 
                "Natation", 
                "Cyclisme", 
                "Course", 
                "Rugby", 
                "Handball", 
                "Volley", 
                "Autre"
            };
        }
    }
}
