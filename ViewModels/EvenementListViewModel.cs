    using UserApp.Models;

    namespace UserApp.ViewModels
    {
        public class EvenementListViewModel
        {
            public IEnumerable<Evenement> Evenements { get; set; }
            public int CurrentPage { get; set; }
            public int TotalPages { get; set; }
        }
    }
