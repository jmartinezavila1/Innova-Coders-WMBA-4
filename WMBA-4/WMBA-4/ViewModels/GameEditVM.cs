using WMBA_4.Models;

namespace WMBA_4.ViewModels
{
    public class GameEditVM
    {
        public Game Game { get; set; }
        public int DivisionID { get; set; }
        public int Team1ID { get; set; }
        public int Team2ID { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }

        public DateTime Date { get; set; }
        public int LocationID { get; set; }
        public int SeasonID { get; set; }
        public int GameTypeID { get; set; }

    }
}
