using System.ComponentModel.DataAnnotations;

namespace WMBA_4.Models
{
    public class ScorePlayer
    {
        public int ID { get; set; }

        [Display(Name = "Hits")]
        public int H { get; set; }

        [Display(Name = "Runs Batted in")]
        public int RBI { get; set; }

        [Display(Name = "1 Base Hit /1B")]
        public int Singles { get; set; }

        [Display(Name = "2 Base Hit /2B")]
        public int Doubles { get; set; }

        [Display(Name = "3 Base Hit /3B")]
        public int Triples { get; set; }

        [Display(Name = "Home Run")]
        public int HR { get; set; }

        [Display(Name = "Base on balls (walks)")]
        public int BB { get; set; }

        [Display(Name = "Plate Appearance")]
        public int PA { get; set; }

        [Display(Name = "At Bat")]
        public int AB { get; set; }

        [Display(Name = "Run")]
        public int Run { get; set; }


        [Display(Name = "Hit by pitch")]
        public int HBP { get; set; }

        [Display(Name = "StrikeOut")]
        public int StrikeOut { get; set; }

        [Display(Name = "Out")]
        public int Out { get; set; }

        [Display(Name = "Fouls")]
        public int Fouls { get; set; }

        [Display(Name = "Balls")]
        public int Balls { get; set; }

        [Display(Name = "Batting Order")]
        public int BattingOrder { get; set; }

        [Display(Name = "Position")]
        public int Position { get; set; }
        public int GameLineUpID { get; set; }
        public GameLineUp GameLineUp { get; set; }


    }
}
