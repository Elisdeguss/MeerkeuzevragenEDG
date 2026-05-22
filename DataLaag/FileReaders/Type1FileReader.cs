using BusinessLaag.Interfaces;
using BusinessLaag.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataLaag.FileReaders
{
    public class Type1FileReader : IFileReader
    {
        public List<Vraag> ExtractQuestionsFromFile(string filePath)
        {
            List<Vraag> vragenUitFile = new List<Vraag>();

            string tempVraagTekst = "";
            Dictionary<string, string> keuzeOpties = new Dictionary<string, string>();
            string correcteAntwoordLetter = "";

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

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                int teller = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim(); 
                    teller++;

                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (char.IsDigit(line[0]))
                    {
                        if (!string.IsNullOrEmpty(tempVraagTekst))
                        {
                            throw new Exception($"Fout in {fileName} op regel {teller}: " +
                                $"Nieuwe vraag gestart terwijl de vorige ('{tempVraagTekst}') nog niet volledig is.");
                        }
                        
                        tempVraagTekst = line.Split(new[] { ". " }, 2, StringSplitOptions.None)[1];
                    }
                    else if (line.Length > 1 && char.IsLetter(line[0]) && line[1] == '.')
                    {
                        string letter = line[0].ToString().ToUpper();
                        string tekst = line.Split(new[] { ". " }, 2, StringSplitOptions.None)[1];
                        keuzeOpties.Add(letter, tekst);
                    }
                    else if (line.StartsWith("Correct:", StringComparison.OrdinalIgnoreCase))
                    {
                        correcteAntwoordLetter = line.Replace("Correct:", "").Trim().ToUpper();

                        if (!keuzeOpties.ContainsKey(correcteAntwoordLetter))
                        {
                            throw new Exception($"Fout in {fileName} op regel {teller}: " +
                                $"Het gegeven antwoord '{correcteAntwoordLetter}' staat niet tussen de opties.");
                        }

                        List<MeerkeuzeOptie> optiesLijst = new List<MeerkeuzeOptie>();
                        foreach (var entry in keuzeOpties)
                        {
                            bool isCorrect = (entry.Key == correcteAntwoordLetter);
                            optiesLijst.Add(new MeerkeuzeOptie(0, entry.Value, isCorrect));
                        }

                        Vraag nieuweVraag = new Vraag(0, onderwerp, tempVraagTekst, moeilijkheidsgraad, true, optiesLijst);
                        vragenUitFile.Add(nieuweVraag);

                        tempVraagTekst = "";
                        keuzeOpties.Clear();
                        correcteAntwoordLetter = "";
                    }
                }
            }

            if (!string.IsNullOrEmpty(tempVraagTekst))
            {
                throw new Exception($"Fout in {fileName}: De laatste vraag in dit bestand is onvolledig ('{tempVraagTekst}').");
            }

            return vragenUitFile;
        }
    }
}