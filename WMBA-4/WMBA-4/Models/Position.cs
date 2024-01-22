namespace WMBA_4.Models
{
    public class Position
    {
        public int ID { get; set; }
        public string PositionCode { get; set; }
        public string PositionName { get; set; }

        public ICollection<GameLineUpPosition> GameLineUpPositions { get; set; } = new HashSet<GameLineUpPosition>();
    }
}
