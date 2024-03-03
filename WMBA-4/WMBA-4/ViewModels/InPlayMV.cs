namespace WMBA_4.ViewModels
{
    public class InPlayMV
    {

        public int InplayID { get; set; }

        public int? PlayerInBase1Base { get; set; }
        public int? PlayerInBase2Base { get; set; }
        public int? PlayerInBase3Base { get; set; }
        public int? PlayerBattingBase { get; set; }
        public bool IsHit { get; set; }

        public bool IsRunPlayer1 { get; set; }

        public bool IsRunPlayer2 { get; set; }

        public bool IsRunPlayer3 { get; set; }
        public bool IsOutPlayer1 { get; set; }
        public bool IsOutPlayer2 { get; set; }
        public bool IsOutPlayer3 { get; set; }
        public int? PlayerBattingId { get; set; }
        public string PlayerBattingName { get; set; }
        public int? PlayerInBase1Id { get; set; }
        public string PlayerInBase1Name { get; set; }
        public int? PlayerInBase2Id { get; set; }
        public string PlayerInBase2Name { get; set; }
        public int? PlayerInBase3Id { get; set; }
        public string PlayerInBase3Name { get; set; }
    }
}
