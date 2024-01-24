using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WMBA_4.Models
{
    public class TeamGame
    {
        [Display(Name = "Is Home Team")]
        public bool IsHomeTeam { get; set; }

        [Display(Name = "Is Visitor Team")]
        public bool IsVisitorTeam { get; set; }

        [Display(Name = "Team")]
        public int TeamID { get; set; }

        public Team Team { get; set; }


        [Display(Name = "Game")]
        public int GameID { get; set; }
        public Game Game { get; set; }
    }
}
