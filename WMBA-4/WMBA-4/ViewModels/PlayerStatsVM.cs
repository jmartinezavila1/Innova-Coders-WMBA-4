using System.ComponentModel.DataAnnotations;
namespace WMBA_4.ViewModels
{
    public class PlayerStatsVM
    {
        public int ID { get; set; }
        public string Player { get; set; }
        public string JerseyNumber { get; set; }
        public string Team { get; set; }
        public int G { get; set; }
        public int H { get; set; }
        public int RBI { get; set; }

        [Display(Name = "1B")]
        public int Singles { get; set; }

        [Display(Name = "2B")]
        public int Doubles { get; set; }

        [Display(Name = "3B")]
        public int Triples { get; set; }
        public int HR { get; set; }
        public int BB { get; set; }
        public int PA { get; set; }
        public int AB { get; set; }
        public int Run { get; set; }
        public int HBP { get; set; }
        public int SO { get; set; }
        public int Out { get; set; }
        public double AVG { get; set; }
        public double OBP { get; set; }
        public double SLG { get; set; }
        public double OPS { get; set; }
    }
}
