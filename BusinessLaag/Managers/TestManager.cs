using BusinessLaag.Exceptions;
using BusinessLaag.Interfaces;
using BusinessLaag.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BusinessLaag.Managers
{
    public class TestManager
    {
        private readonly IVraagRepository _vraagRepo;

        public TestManager(IVraagRepository vraagRepo)
        {
            _vraagRepo = vraagRepo ?? throw new ArgumentNullException(nameof(vraagRepo));
        }

        
        /// <param name="onderwerp">Het onderwerp waaruit vragen gehaald worden.</param>
        /// <param name="aantal">Hoeveel vragen er nodig zijn.</param>
        /// <param name="moeilijkheid">"Gemengd" = alle niveaus, anders exact filteren.</param>
        /// <param name="reedsGekozen">Vragen die al in de test zitten (worden uitgesloten).</param>
        public List<Vraag> GenereerTestVragen(
            string onderwerp,
            int aantal,
            string moeilijkheid = "Gemengd",
            List<Vraag> reedsGekozen = null)
        {
            
            if (string.IsNullOrWhiteSpace(onderwerp))
                throw new VraagExceptions("Onderwerp mag niet leeg zijn.");

            if (aantal <= 0)
                throw new VraagExceptions("Het aantal vragen moet groter zijn dan 0.");

            
            var alleVragen = _vraagRepo.GetVragenByOnderwerp(onderwerp, alleenActief: true);

            
            var beschikbaar = alleVragen
                .Where(v => moeilijkheid == "Gemengd" || v.Moeilijkheidsgraad == moeilijkheid)
                .Where(v => reedsGekozen == null || !reedsGekozen.Any(r => r.Id == v.Id))
                .ToList();

            
            if (beschikbaar.Count < aantal)
                throw new VraagExceptions(
                    $"Niet genoeg vragen beschikbaar voor '{onderwerp}' ({moeilijkheid}).\n" +
                    $"Gevraagd: {aantal}, nog beschikbaar: {beschikbaar.Count}.");

            
            var geselecteerd = beschikbaar
                .OrderBy(_ => Guid.NewGuid())
                .Take(aantal)
                .ToList();

            
            foreach (var vraag in geselecteerd)
            {
                vraag.Keuzes = HusselAntwoorden(vraag.Keuzes);
            }

            return geselecteerd;
        }

        
        public void OpslaanEnExporteerTest(string testCode, List<Vraag> vragen, string opslagPad)
        {
            
            if (string.IsNullOrWhiteSpace(testCode))
                throw new VraagExceptions("Testcode mag niet leeg zijn.");

            if (vragen == null || vragen.Count == 0)
                throw new VraagExceptions("Een test moet minstens één vraag bevatten.");

            
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
                    char optieLetter = 'A';
                    foreach (var optie in vraag.Keuzes)
                    {
                        if (optie.IsCorrect)
                        {
                            swKey.WriteLine($"Vraag {vraagNummer}: {optieLetter}) {optie.Tekst}");
                            break;
                        }
                        optieLetter++;
                    }
                    vraagNummer++;
                }
            }
        }

        

        private List<MeerkeuzeOptie> HusselAntwoorden(List<MeerkeuzeOptie> opties)
        {
            return opties.OrderBy(_ => Guid.NewGuid()).ToList();
        }
    }
}