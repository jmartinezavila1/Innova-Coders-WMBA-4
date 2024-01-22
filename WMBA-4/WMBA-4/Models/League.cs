namespace WMBA_4.Models
{
    public class League
    {
        public int ID { get; set; }
        public string LeagueName { get; set; }
        public int EstablishYear { get; set;}

        public int CityID { get; set; }

        public City City { get; set; }

        public ICollection<Division> Divisions { get; set; } = new HashSet<Division>();

    }
}
