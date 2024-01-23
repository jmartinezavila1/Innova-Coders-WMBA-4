namespace WMBA_4.Models
{
    public class Season
    {
        public int ID { get; set; }
        public string SeasonCode { get; set; }
        public string SeasonName { get; set; }

        public ICollection<Game> Games { get; set; } = new HashSet<Game>();
    }
}
