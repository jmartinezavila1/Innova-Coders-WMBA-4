using System.ComponentModel.DataAnnotations;

namespace WMBA_4.Models
{
    public class GameType
    {
        public int ID { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "You cannot leave the Game Type description blank.")]
        [StringLength(150, ErrorMessage = "Description cannot be more than 150 characters long.")]
        public string Description { get; set; }

        public ICollection<Game> Games { get; set; } = new HashSet<Game>();
    }
}
