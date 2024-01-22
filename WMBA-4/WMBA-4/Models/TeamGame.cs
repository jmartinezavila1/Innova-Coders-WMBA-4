using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WMBA_4.Models
{
    public class TeamGame
    {       
        public bool IsHomeTeam { get; set; }

        public bool IsVisitorTeam { get; set;}

        [Display(Name = "Team")]
        public int TeamID { get; set; }

        public Team Team { get; set; }


        [Display(Name = "Game")]
        public int GameID { get; set; }
        public Game Game { get; set; }
    }
}
