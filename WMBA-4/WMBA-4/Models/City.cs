namespace WMBA_4.Models
{
    public class City
    {
        public int ID { get; set; }
        public string CityName { get; set; }
        public ICollection<League> Leagues { get; set; } = new HashSet<League>();
        public ICollection<Location> Locations { get; set; } = new HashSet<Location>();
    }
}
