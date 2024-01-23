using System.ComponentModel.DataAnnotations;

namespace WMBA_4.Models
{
    public class Season
    {
        public int ID { get; set; }

        [Display(Name = "Season Code")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "The Season code must be exactly  3 characters long.")]

        public string SeasonCode { get; set; }

        [Display(Name = "Season Name")]
        [Required(ErrorMessage = "You cannot leave the Season name blank.")]
        [StringLength(10, ErrorMessage = "Season name cannot be more than 10 characters long.")]
        public string SeasonName { get; set; }

        public ICollection<Game> Games { get; set; } = new HashSet<Game>();
    }
}
