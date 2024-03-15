using System.ComponentModel.DataAnnotations;
using WMBA_4.Models;

namespace WMBA_4.ViewModels
{
    public class GameEditVM
    {
        public Game Game { get; set; }
        [Display(Name = "Division")]
        public int DivisionID { get; set; }
        [Display(Name = "Home Team")]
        public int Team1ID { get; set; }
        [Display(Name = "Visitor Team")]
        public int Team2ID { get; set; }
        public string Team1Name { get; set; }
        public string Team2Name { get; set; }
        public int? Score1 { get; set; }
        public int? Score2 { get; set; }
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
        [Display(Name = "Location")]
        public int LocationID { get; set; }
        [Display(Name = "Season")]
        public int SeasonID { get; set; }
        [Display(Name = "Game Type")]
        public int GameTypeID { get; set; }

        [Display(Name = "Location")]
        public ICollection<Location> Locations { get; set; } = new HashSet<Location>();
        public ICollection<Division> Divisions { get; set; } = new HashSet<Division>();
        public ICollection<Team> Teams { get; set; } = new HashSet<Team>();
        public ICollection<Season> Seasons { get; set; } = new HashSet<Season>();
        public ICollection<GameType> GameTypes { get; set; } = new HashSet<GameType>();
        
}
}
