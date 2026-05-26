using BusinessLaag.Exceptions;

namespace BusinessLaag.Model
{
    public class Vraag
    {
      
        public Vraag(string onderwerp, string inhoud, string moeilijkheidsgraad, bool isActief, List<MeerkeuzeOptie> keuzes)
        {
            Onderwerp = onderwerp;
            Inhoud = inhoud;
            Moeilijkheidsgraad = moeilijkheidsgraad;
            IsActief = isActief;
            Keuzes = keuzes;
        }

     
        public Vraag(int id, string onderwerp, string inhoud, string moeilijkheidsgraad, bool isActief, List<MeerkeuzeOptie> keuzes)
        {
            Id = id;
            Onderwerp = onderwerp;
            Inhoud = inhoud;
            Moeilijkheidsgraad = moeilijkheidsgraad;
            IsActief = isActief;
            Keuzes = keuzes;
        }

        public int Id { get; set; }

        private string _onderwerp;
        public string Onderwerp
        {
            get { return _onderwerp; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new VraagExceptions("Onderwerp mag niet leeg zijn.");
                if (value.Trim() != value)
                    throw new VraagExceptions("Onderwerp mag niet beginnen of eindigen met spaties.");
                _onderwerp = value;
            }
        }

        private string _inhoud;
        public string Inhoud
        {
            get { return _inhoud; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new VraagExceptions("Inhoud mag niet leeg zijn.");
                if (value.Trim() != value)
                    throw new VraagExceptions("Inhoud mag niet beginnen of eindigen met spaties.");
                _inhoud = value;
            }
        }

        private string _moeilijkheidsgraad;
        public string Moeilijkheidsgraad
        {
            get { return _moeilijkheidsgraad; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new VraagExceptions("Moeilijkheidsgraad mag niet leeg zijn.");
                _moeilijkheidsgraad = value;
            }
        }

        public bool IsActief { get; set; }

        private List<MeerkeuzeOptie> _keuzes;
        public List<MeerkeuzeOptie> Keuzes
        {
            get { return _keuzes; }
            set
            {
                if (value == null || value.Count < 2)
                    throw new VraagExceptions("Een vraag moet minstens 2 antwoordopties hebben.");
                if (!value.Any(k => k.IsCorrect))
                    throw new VraagExceptions("Een vraag moet minstens één correct antwoord hebben.");
                _keuzes = value;
            }
        }
    }
}