using System.ComponentModel.DataAnnotations;

namespace WMBA_4.Models
{
    public class Game
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "You must enter a date and time for the Game.")]
        [Display(Name = "Date")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Display(Name = "Status")]
        public bool Status { get; set; } = true;

        [Display(Name = "Location")]
        public int LocationID { get; set; }

        public Location Location { get; set; }

        [Display(Name = "Season")]
        public int SeasonID { get; set; }

        public Season Season { get; set; }


        [Display(Name = "GameType")]
        public int GameTypeID { get; set; }
        public GameType GameType { get; set; }


        public ICollection<TeamGame> TeamGames { get; set; } = new HashSet<TeamGame>();

        public ICollection<GameLineUp> GameLineUps { get; set; } = new HashSet<GameLineUp>();

        public ICollection<ScorePlayer> ScorePlayers { get; set; } = new HashSet<ScorePlayer>();


    }
}
