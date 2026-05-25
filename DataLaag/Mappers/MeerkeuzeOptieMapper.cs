using BusinessLaag.Model;
using DataLaag.ModelUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLaag.Mappers
{
    public static class MeerkeuzeOptieMapper
    {

        public static MeerkeuzeOptieUI MapNaarUI(MeerkeuzeOptie o)
        {
            return new MeerkeuzeOptieUI(o.Id, o.Tekst, o.IsCorrect);
        }

        public static MeerkeuzeOptie MapNaarDomein(MeerkeuzeOptieUI o)
        {
            return new MeerkeuzeOptie(o.Id, o.Tekst, o.IsCorrect);
        }
    }
}
