namespace WMBA_4.ViewModels
{
    public class GameEndSeasonVM
    {
        public string ID { get; set; }

        public string Date { get; set; }

        public string LocationName { get; set; }

        public string DivisionName { get; set; }

        public string TeamHome { get; set; }
        public int scoreHome { get; set; }

        public string TeamVisitor { get; set; }

        public int scoreVisitor { get; set; }

        public bool Status { get; set; }

    }
}
