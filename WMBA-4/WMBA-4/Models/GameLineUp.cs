using System.ComponentModel.DataAnnotations;

namespace WMBA_4.Models
{
    public class GameLineUp
    {
        public int ID { get; set; }

        [Display(Name = "Batting Order")]
        [Range(1, 9, ErrorMessage = "Please enter a number between 1 and 9.")]
        public int BattingOrder { get; set; }

        [Display(Name = "Game")]
        public int GameID { get; set; }

        public Game Game { get; set; }

        [Display(Name = "Player")]
        public int PlayerID { get; set; }

        public Player Player { get; set; }

        [Display(Name = "Team")]
        public int TeamID { get; set; }

        public Team Team { get; set; }

        public ICollection<GameLineUpPosition> GameLineUpPositions { get; set; } = new HashSet<GameLineUpPosition>();

        public ICollection<ScorePlayer> ScoresPlayer { get; set; } = new HashSet<ScorePlayer>();

    }
}
