using System.ComponentModel.DataAnnotations;

namespace WMBA_4.Models
{
    public class Location
    {
        public int ID { get; set; }

        [Display(Name = "Location")]
        [Required(ErrorMessage = "You cannot leave the Location name blank.")]
        [StringLength(150, ErrorMessage = "Location name cannot be more than 150 characters long.")]
        public string LocationName { get; set; }

        [Display(Name = "City")]
        public int CityID { get; set; }

        public City City { get; set; }

        public ICollection<Game> Games { get; set; } = new HashSet<Game>();


    }
}
