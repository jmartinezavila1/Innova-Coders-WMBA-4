using NuGet.DependencyResolver;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMBA_4.Models
{
    public class Inplay
    {
        public int ID { get; set; }
        public int Runs { get; set; }

        public int Strikes { get; set; }

        public int Outs { get; set; }

        public int Fouls { get; set; }
        public int Balls { get; set; }

        public int TeamAtBat { get; set; }
        public int Turns { get; set; }
        public int OpponentOuts { get; set; }
        public int InningID { get; set; }
        public Inning Inning { get; set; }
        public int NextPlayer { get; set; }

        /// <summary>
        /// Inverse properties for the player in the base
        /// </summary>

        [ForeignKey("PlayerInBase1")]
        public int? PlayerInBase1ID { get; set; }
        public Player PlayerInBase1 { get; set; }

        [ForeignKey("PlayerInBase2")]
        public int? PlayerInBase2ID { get; set; }
        public Player PlayerInBase2 { get; set; }

        [ForeignKey("PlayerInBase3")]
        public int? PlayerInBase3ID { get; set; }
        public Player PlayerInBase3 { get; set; }

        [ForeignKey("PlayerBatting")]
        public int? PlayerBattingID { get; set; }
        public Player PlayerBatting { get; set; }


    }
}
