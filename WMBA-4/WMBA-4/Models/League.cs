using System.ComponentModel.DataAnnotations;

namespace WMBA_4.Models
{
    public class League
    {
        public int ID { get; set; }

        [Display(Name = "League Name")]
        [Required(ErrorMessage = "You cannot leave the League name blank.")]
        [StringLength(100, ErrorMessage = "League name cannot be more than 100 characters long.")]
        public string LeagueName { get; set; }

        [Display(Name = "Established Year")]
        [RegularExpression("^\\d{4}$", ErrorMessage = "The Stablished year  must be exactly the 4 digits of the year.")]
        [StringLength(4)]
        public int EstablishYear { get; set;}

        [Display(Name = "City")]
        public int CityID { get; set; }

        public City City { get; set; }

        public ICollection<Division> Divisions { get; set; } = new HashSet<Division>();

    }
}
