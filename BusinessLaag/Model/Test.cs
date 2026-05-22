using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLaag.Model
{
    public class Test
    {
        public Test(int id, string code, DateTime aanmaakDatum, List<Vraag> vragenInTest, List<string> onderwerpen)
        {
            Id = id;
            Code = code;
            AanmaakDatum = aanmaakDatum;
            VragenInTest = vragenInTest;
            Onderwerpen = onderwerpen;
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime AanmaakDatum { get; set; }

        public List<Vraag> VragenInTest { get; set; }
        public List<string> Onderwerpen { get; set; }


        public List<TestResultaat> DeelnemendeStudentenResultaten { get; set; } = new List<TestResultaat>();


        public double KlasGemiddelde
        {
            get
            {
                if (DeelnemendeStudentenResultaten == null || DeelnemendeStudentenResultaten.Count == 0) return 0;

                
                return Math.Round(DeelnemendeStudentenResultaten.Average(r => r.Score), 1);
            }
        }

        
        public string GemiddeldeWeergave
        {
            get
            {
                
                int aantalVragen = VragenInTest?.Count ?? 0;

                return $"{KlasGemiddelde} / {aantalVragen}";
            }
        }

        public override string ToString()
        {
            return Code;
        }
    }
}
