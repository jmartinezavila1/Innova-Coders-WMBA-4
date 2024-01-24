using System.ComponentModel.DataAnnotations;

namespace WMBA_4.Models
{
    public class Position
    {
        public int ID { get; set; }

        [Display(Name = "Position Code")]
        [Required(ErrorMessage = "You cannot leave the Position code blank.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "The Position code must be exactly  2 characters long.")]

        public string PositionCode { get; set; }

        [Display(Name = "Position Name")]
        [Required(ErrorMessage = "You cannot leave the Position name blank.")]
        [StringLength(50, ErrorMessage = "Position name cannot be more than 50 characters long.")]
        public string PositionName { get; set; }

        public ICollection<GameLineUpPosition> GameLineUpPositions { get; set; } = new HashSet<GameLineUpPosition>();
    }
}
