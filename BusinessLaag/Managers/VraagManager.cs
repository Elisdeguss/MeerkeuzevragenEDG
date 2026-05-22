using BusinessLaag.Exceptions;
using BusinessLaag.Interfaces;
using BusinessLaag.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLaag.Managers
{
    public class VraagManager
    {
        private readonly IVraagRepository _vraagRepo;
        private readonly IFileReaderFactory _fileReaderFactory;

        public VraagManager(IVraagRepository vraagRepo, IFileReaderFactory fileReaderFactory)
        {
            _vraagRepo = vraagRepo ?? throw new ArgumentNullException(nameof(vraagRepo));
            _fileReaderFactory = fileReaderFactory ?? throw new ArgumentNullException(nameof(fileReaderFactory));
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
    }
}