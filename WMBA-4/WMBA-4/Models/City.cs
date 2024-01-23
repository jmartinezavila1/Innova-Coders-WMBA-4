using System.ComponentModel.DataAnnotations;

namespace WMBA_4.Models
{
    public class City
    {
        public int ID { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "You must enter the City name.")]
        [StringLength(300, ErrorMessage = "City cannot be more than 300 characters long.")]
        public string CityName { get; set; }
        public ICollection<League> Leagues { get; set; } = new HashSet<League>();
        public ICollection<Location> Locations { get; set; } = new HashSet<Location>();
    }
}
