using System.ComponentModel.DataAnnotations;

namespace WMBA_4.Models
{
    public class Club
    {
        public int ID { get; set; }

        [Display(Name = "Club Name")]
        [Required(ErrorMessage = "You cannot leave the Club name blank.")]
        [StringLength(100, ErrorMessage = "Club name cannot be more than 100 characters long.")]
        public string ClubName { get; set; }

        [Display(Name = "Status")]
        public bool Status { get; set; } = true;

        [Display(Name = "City")]
        public int CityID { get; set; }

        public City City { get; set; }

        public ICollection<Division> Divisions { get; set; } = new HashSet<Division>();
    }
}
