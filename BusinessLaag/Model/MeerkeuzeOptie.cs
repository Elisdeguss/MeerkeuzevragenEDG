namespace BusinessLaag.Model
{
    public class MeerkeuzeOptie
    {
        public MeerkeuzeOptie(string tekst)
        {

            Tekst = tekst;
            IsCorrect = false;


        }

        public MeerkeuzeOptie(int id, string tekst, bool isCorrect)
        {
            Id = id;

            Tekst = tekst;
            IsCorrect = isCorrect;
        }

        public int Id { get; set; }

        public string Tekst { get; set; }
        public bool IsCorrect { get; set; }

    }
}
