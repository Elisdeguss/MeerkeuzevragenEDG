using BusinessLaag.Interfaces;
using BusinessLaag.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLaag.Managers
{
    public class VraagManager
    {

        private IVraagRepository _vraagRepo;
        private readonly IFileReaderFactory _fileReaderFactory;

        public VraagManager(IVraagRepository vraagRepo, IFileReaderFactory fileReaderFactory)
        {
            _vraagRepo = vraagRepo;
            _fileReaderFactory = fileReaderFactory;
        }

        public void ImporteerBestand(string path)
        {
            
            IFileReader geselecteerdeReader = _fileReaderFactory.GetReader(path);

            
            var vragen = geselecteerdeReader.ExtractQuestionsFromFile(path);

            foreach (var v in vragen)
            {
                _vraagRepo.VoegVraagToe(v);
            }
        }

        public List<Vraag> GenereerTestVragen(string onderwerp, int aantal, bool alleenActief = true)
        {
            
            List<Vraag> alleVragen = _vraagRepo.GetVragenByOnderwerp(onderwerp, alleenActief);

            
            Random rng = new Random();
            var geselecteerdeVragen = alleVragen
                .OrderBy(x => rng.Next())
                .Take(aantal)
                .ToList();

            
            foreach (var vraag in geselecteerdeVragen)
            {
                vraag.Keuzes = HusselAntwoorden(vraag.Keuzes);
            }

            return geselecteerdeVragen;
        }

        
        private List<MeerkeuzeOptie> HusselAntwoorden(List<MeerkeuzeOptie> opties)
        {
            Random rng = new Random();
            return opties.OrderBy(x => rng.Next()).ToList();
        }

        public void ArchiveerVraag(int id)
        {
            
            _vraagRepo.DeactiveerVraag(id);
        }

        public void OpslaanEnExporteerTest(string testCode, List<Vraag> vragen, string opslagPad)
        {
            
            _vraagRepo.OpslaanTest(testCode, vragen);

            
            using (StreamWriter swTest = new StreamWriter(opslagPad, false, Encoding.UTF8))
            {
                swTest.WriteLine("========================================");
                swTest.WriteLine($"MEERKEUZETEST: {testCode}");
                swTest.WriteLine($"Datum gegenereerd: {DateTime.Now.ToShortDateString()}");
                swTest.WriteLine("========================================\n");

                int vraagNummer = 1;
                foreach (var vraag in vragen)
                {
                    swTest.WriteLine($"Vraag {vraagNummer}: {vraag.Inhoud}");

                    char optieLetter = 'A';
                    foreach (var optie in vraag.Keuzes)
                    {
                        swTest.WriteLine($"  {optieLetter}) {optie.Tekst}");
                        optieLetter++;
                    }

                    swTest.WriteLine();
                    vraagNummer++;
                }
            }

            
            string mapPad = Path.GetDirectoryName(opslagPad);
            string bestandNaam = Path.GetFileNameWithoutExtension(opslagPad);
            string extensie = Path.GetExtension(opslagPad);

            string antwoordKeyPad = Path.Combine(mapPad, $"{bestandNaam}_Oplossingen{extensie}");

            
            using (StreamWriter swKey = new StreamWriter(antwoordKeyPad, false, Encoding.UTF8))
            {
                swKey.WriteLine("========================================");
                swKey.WriteLine($"ANTWOORDSLEUTEL / OPLOSSINGEN: {testCode}");
                swKey.WriteLine($"Datum gegenereerd: {DateTime.Now.ToShortDateString()}");
                swKey.WriteLine("========================================\n");

                int vraagNummer = 1;
                foreach (var vraag in vragen)
                {
                    char correcteLetter = ' ';
                    string correcteTekst = "";

                    
                    char optieLetter = 'A';
                    foreach (var optie in vraag.Keuzes)
                    {
                        if (optie.IsCorrect)
                        {
                            correcteLetter = optieLetter;
                            correcteTekst = optie.Tekst;
                            break; 
                        }
                        optieLetter++;
                    }

                    
                    swKey.WriteLine($"Vraag {vraagNummer}: {correcteLetter}) {correcteTekst}");
                    vraagNummer++;
                }
            }
        }
    }
}

    
    

