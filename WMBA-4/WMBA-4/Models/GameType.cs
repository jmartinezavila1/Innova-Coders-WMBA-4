namespace WMBA_4.Models
{
    public class GameType
    {
        public int ID { get; set; }
        public string Description { get; set; }

        public ICollection<Game> Games { get; set; } = new HashSet<Game>();
    }
}
