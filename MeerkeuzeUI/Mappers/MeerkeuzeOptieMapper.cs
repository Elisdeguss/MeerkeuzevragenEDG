using BusinessLaag.Model;
using MeerkeuzeUI.ModelUI;

namespace MeerkeuzeUI.Mappers
{
    public static class MeerkeuzeOptieMapper
    {
        
        public static MeerkeuzeOptieUI MapNaarUI(MeerkeuzeOptie o, int vraagId)
        {
            return new MeerkeuzeOptieUI(o.Id, o.Tekst, o.IsCorrect, vraagId);
        }

        public static MeerkeuzeOptie MapNaarDomein(MeerkeuzeOptieUI o)
        {
            return new MeerkeuzeOptie(o.Id, o.Tekst, o.IsCorrect);
        }
    }
}