namespace MeerkeuzeUI.ModelUI
{

    public class ScoreRijUI
    {
        public string StudentNaam { get; set; }
        public string TestCode { get; set; }
        public int BehaaldePunten { get; set; }
        public int TotaalVragen { get; set; }

        public int TotaalPuntenAlleTests { get; set; }
        public int TotaalVragenAlleTests { get; set; }


        public double ScoreOp20 => TotaalVragen == 0
            ? 0
            : Math.Round((double)BehaaldePunten / TotaalVragen * 20, 1);

        public string ScoreWeergave => $"{BehaaldePunten} / {TotaalVragen}";


        public string ScoreOp20Weergave => TotaalVragenAlleTests == 0
    ? "0 / 20"
    : $"{Math.Round((double)TotaalPuntenAlleTests / TotaalVragenAlleTests * 20, 1)} / 20";
    }
}