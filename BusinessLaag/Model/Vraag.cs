using System;
using System.Collections.Generic;
using System.Text;

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
        public string Onderwerp { get; set; }

        public string Inhoud { get; set; }  

        public string Moeilijkheidsgraad { get; set; }

        public bool IsActief { get; set; } 

        public List<MeerkeuzeOptie> Keuzes { get; set; }

        




    }
}
