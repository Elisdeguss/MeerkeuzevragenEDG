using BusinessLaag.Interfaces;
using BusinessLaag.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLaag.FileReaders
{
    public class Type2FileReader : IFileReader
    {
        List<Vraag> IFileReader.ExtractQuestionsFromFile(string filePath)
        {
            List<Vraag> vragenUitFile = new List<Vraag>();

            string tempVraagTekst = "";
           
            List<string> correcteAntwoorden = new List<string>();
            List<string> tempOpties = new List<string>();
            bool isBinnenVraag = false;
            bool isInAntwoordenSleutel = false;


            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string moeilijkheidsgraad = "Open level";

            if (fileName.Contains("Expert", StringComparison.OrdinalIgnoreCase))
            {
                moeilijkheidsgraad = "Expert";
            }
            else if (fileName.Contains("Beginner", StringComparison.OrdinalIgnoreCase))
            {
                moeilijkheidsgraad = "Beginner";
            }

            string onderwerp = fileName;
            onderwerp = onderwerp.Replace("Expert", "", StringComparison.OrdinalIgnoreCase);
            onderwerp = onderwerp.Replace("Beginner", "", StringComparison.OrdinalIgnoreCase);
            onderwerp = onderwerp.Replace("Open level", "", StringComparison.OrdinalIgnoreCase);
            onderwerp = onderwerp.Trim();


            if (onderwerp.Contains('_'))
            {
                onderwerp = onderwerp.Split('_')[0].Trim();
            }
            else
            {
                if (onderwerp.Length > 1 &&
                    char.IsDigit(onderwerp[onderwerp.Length - 1]) &&
                    !char.IsDigit(onderwerp[onderwerp.Length - 2]))
                {

                    onderwerp = onderwerp.Substring(0, onderwerp.Length - 1);
                }

                
            }
            
            

            using (StreamReader sr = new StreamReader(filePath))
            {
                string regel;
                while ((regel = sr.ReadLine()) != null)
                {
                    regel = regel.Trim();
                    if (string.IsNullOrWhiteSpace(regel)) continue;

                    
                    if (regel.Equals("Antwoorden", StringComparison.OrdinalIgnoreCase))
                    {
                        
                        if (isBinnenVraag && tempOpties.Count > 0)
                        {
                            vragenUitFile.Add(BouwVraagBasis(tempVraagTekst, tempOpties, onderwerp, moeilijkheidsgraad));
                            tempOpties.Clear();
                        }
                        isInAntwoordenSleutel = true;
                        continue;
                    }

                    
                    if (isInAntwoordenSleutel)
                    {
                        if ("ABCD".Contains(char.ToUpper(regel[0])))
                        {
                            correcteAntwoorden.Add(char.ToUpper(regel[0]).ToString());
                        }
                    }
                    
                    else
                    {
                        
                        if (IsNieuweVraagRegel(regel, out string vraagInhoud))
                        {
                            
                            if (isBinnenVraag && tempOpties.Count > 0)
                            {
                                vragenUitFile.Add(BouwVraagBasis(tempVraagTekst, tempOpties, onderwerp, moeilijkheidsgraad));
                                tempOpties.Clear();
                            }

                            tempVraagTekst = vraagInhoud;
                            isBinnenVraag = true;
                        }
                        
                        else if (isBinnenVraag && regel.Length > 1 && "ABCD".Contains(char.ToUpper(regel[0])) && (regel[1] == '.' || regel[1] == ')'))
                        {
                            string optieTekst = regel.Substring(2).Trim();
                            tempOpties.Add(optieTekst);
                        }
                        
                        else if (isBinnenVraag && tempOpties.Count == 0)
                        {
                            if (string.IsNullOrEmpty(tempVraagTekst)) tempVraagTekst = regel;
                            else tempVraagTekst += Environment.NewLine + regel;
                        }

                     
                    }
                }
            }

            
            for (int i = 0; i < vragenUitFile.Count; i++)
            {
                if (i < correcteAntwoorden.Count)
                {
                    string juisteLetter = correcteAntwoorden[i];
                    int juisteIndex = juisteLetter[0] - 'A';

                    if (juisteIndex >= 0 && juisteIndex < vragenUitFile[i].Keuzes.Count)
                    {
                        vragenUitFile[i].Keuzes[juisteIndex].IsCorrect = true;
                    }
                }
            }

            return vragenUitFile;
        }

        private bool IsNieuweVraagRegel(string regel, out string inhoud)
        {
            inhoud = "";
            if (string.IsNullOrEmpty(regel) || !char.IsDigit(regel[0])) return false;

            int puntIndex = regel.IndexOf('.');
            if (puntIndex > 0)
            {
                string nummerDeel = regel.Substring(0, puntIndex);
                foreach (char c in nummerDeel) { if (!char.IsDigit(c)) return false; }

                inhoud = regel.Substring(puntIndex + 1).Trim();
                return true;
            }
            return false;
        }

        
        private Vraag BouwVraagBasis(string inhoud, List<string> opties, string onderwerp, string moeilijkheid)
        {
            Vraag v = new Vraag(onderwerp, inhoud, moeilijkheid, true, new List<MeerkeuzeOptie>())
            {
                Inhoud = inhoud,
                Onderwerp = onderwerp,
                Moeilijkheidsgraad = moeilijkheid,
                IsActief = true,
                Keuzes = new List<MeerkeuzeOptie>()
            };

            foreach (string optieTekst in opties)
            {
                v.Keuzes.Add(new MeerkeuzeOptie(optieTekst) { Tekst = optieTekst, IsCorrect = false });
            }
            return v;
        }
    }
}

