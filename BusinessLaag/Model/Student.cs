using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLaag.Model
{
    public class Student
    {
        public Student(int id, string naam)
        {
            Id = id;
            Naam = naam;
            AlleResultaten = new List<TestResultaat>();
        }

        public int Id { get; set; }
        public string Naam { get; set; }


        public List<TestResultaat> AlleResultaten { get; set; }

        
        public double EindscoreOp20
        {
            get
            {
                if (AlleResultaten == null || AlleResultaten.Count == 0)
                    return 0;

                
                int totaalCorrect = AlleResultaten.Sum(r => r.Score);
                int totaalVragen = AlleResultaten.Sum(r => r.TotaalVragen);

                if (totaalVragen == 0)
                    return 0;

                double eindscore = ((double)totaalCorrect / totaalVragen) * 20;
                return Math.Round(eindscore, 1);
            }
        }


    }
}
