using BusinessLaag.Exceptions;
using BusinessLaag.Interfaces;
using BusinessLaag.Model;

namespace BusinessLaag.Managers
{
    public class VraagManager
    {
        private readonly IVraagRepository _vraagRepo;
        private readonly IFileReaderFactory _fileReaderFactory;

        public VraagManager(IVraagRepository vraagRepo, IFileReaderFactory fileReaderFactory)
        {
            _vraagRepo = vraagRepo;
            _fileReaderFactory = fileReaderFactory;
        }


        public void ImporteerBestand(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new VraagExceptions("Bestandspad mag niet leeg zijn.");

            IFileReader geselecteerdeReader = _fileReaderFactory.GetReader(path);
            var vragen = geselecteerdeReader.ExtractQuestionsFromFile(path);

            foreach (var v in vragen)
            {
                _vraagRepo.VoegVraagToe(v);
            }
        }


        public void VoegNieuweVraagToe(Vraag vraag)
        {
            if (vraag == null)
                throw new VraagExceptions("Vraag mag niet null zijn.");

            if (string.IsNullOrWhiteSpace(vraag.Inhoud))
                throw new VraagExceptions("De inhoud van een vraag mag niet leeg zijn.");

            if (string.IsNullOrWhiteSpace(vraag.Onderwerp))
                throw new VraagExceptions("Een vraag moet een onderwerp hebben.");

            if (vraag.Keuzes == null || vraag.Keuzes.Count < 2)
                throw new VraagExceptions("Een vraag moet minstens 2 antwoordopties hebben.");

            if (!vraag.Keuzes.Any(k => k.IsCorrect))
                throw new VraagExceptions("Een vraag moet minstens één correct antwoord hebben.");

            _vraagRepo.VoegVraagToe(vraag);
        }


        public void ArchiveerVraag(int id)
        {
            if (id <= 0)
                throw new VraagExceptions("Ongeldig vraag-ID.");

            _vraagRepo.DeactiveerVraag(id);
        }


        public List<string> GetAlleOnderwerpen()
        {
            return _vraagRepo.GetAlleOnderwerpen();
        }

        public List<Vraag> GetVragenByOnderwerp(string onderwerp, string moeilijkheid = "Gemengd", bool alleenActief = true)
        {
            if (string.IsNullOrWhiteSpace(onderwerp))
                throw new VraagExceptions("Onderwerp mag niet leeg zijn.");

            var alleVragen = _vraagRepo.GetVragenByOnderwerp(onderwerp, alleenActief: true);

            return alleVragen
                .Where(v => moeilijkheid == "Gemengd" || v.Moeilijkheidsgraad == moeilijkheid)
                .ToList();
        }
    }
}