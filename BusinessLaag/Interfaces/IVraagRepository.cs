using BusinessLaag.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLaag.Interfaces
{
    public interface IVraagRepository
    {

        List<Vraag> GetVragenByOnderwerp(string onderwerp, bool alleenActief);

        
        void VoegVraagToe(Vraag vraag);

        
        void DeactiveerVraag(int vraagId);
        List<string> GetAlleOnderwerpen();

        


    }
}
