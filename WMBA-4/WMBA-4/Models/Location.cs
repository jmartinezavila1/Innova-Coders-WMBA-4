using System.ComponentModel.DataAnnotations;

namespace WMBA_4.Models
{
    public class Location
    {
        public int ID { get; set; }
        public string LocationName { get; set; }

        public int CityID { get; set; }

        public City City { get; set; }

        public ICollection<Game> Games { get; set; } = new HashSet<Game>();


    }
}
