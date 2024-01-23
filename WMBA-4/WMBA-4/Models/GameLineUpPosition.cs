using System.ComponentModel.DataAnnotations;

namespace WMBA_4.Models
{
    public class GameLineUpPosition
    {
       
        [Display(Name = "Line Up")]
        public int GameLineUpID { get; set; }

        public GameLineUp GameLineUp { get; set; }

        [Display(Name = "Position")]
        public int PositionID { get; set; }

        public Position Position { get; set; }
    }
}
