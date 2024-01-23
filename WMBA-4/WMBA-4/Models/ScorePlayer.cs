using System.ComponentModel.DataAnnotations;

namespace WMBA_4.Models
{
    public class ScorePlayer
    {
        public int ID { get; set; }
        public int InningNumber { get; set; }

        [Display(Name = "Hits")]
        public int H { get; set; }

        [Display(Name = "Runs Batted in")]
        public int RBI { get; set; }

        [Display(Name = "Runs")]
        public int R { get; set; }

        [Display(Name = "StrikeOut")]
        public int StrikeOut { get; set; }

        [Display(Name = "GroundOut")]
        public int GroundOut { get; set; }

        [Display(Name = "PopOut")]
        public int PopOut { get; set; }

        [Display(Name = "Flyout")]
        public int Flyout { get; set; }

        [Display(Name = "1 Base Hit /1B")]
        public int Singles { get; set; }

        [Display(Name = "2 Base Hit /2B")]
        public int Doubles{ get; set; }

        [Display(Name = "3 Base Hit /3B")]
        public int Triples { get; set; }

        [Display(Name = "Home Run")]
        public int HR { get; set; }

        [Display(Name = "Base on balls (walks)")]
        public int BB { get; set; }

        [Display(Name = "Hit by pitch")]
        public int HBP { get; set; }

        [Display(Name = "Stolen Base")]
        public int SB { get; set; }

        [Display(Name = "Sacrifice")]
        public int SAC { get; set; }

        [Display(Name = "Plate Appearance")]
        public int PA { get; set; }

        [Display(Name = "At Bat")]
        public int AB { get; set; }

        public int GameID { get; set; }
        public Game Game { get; set; }

        public int PlayerID { get; set; }
        public Player Player { get; set; }




    }
}
