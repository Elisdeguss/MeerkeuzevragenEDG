using BusinessLaag.Model;
using MeerkeuzeUI.ModelUI;

namespace MeerkeuzeUI.Mappers
{
    public static class VraagMapper
    {
        public static VraagUI MapNaarUI(Vraag v)
        {
            return new VraagUI(
                v.Id,
                v.Onderwerp,
                v.Inhoud,
                v.Moeilijkheidsgraad,
                v.IsActief,
                
                v.Keuzes.Select(k => MeerkeuzeOptieMapper.MapNaarUI(k, v.Id)).ToList()
            );
        }

        public static Vraag MapNaarDomein(VraagUI v)
        {
            return new Vraag(
                v.Id,
                v.Onderwerp,
                v.Inhoud,
                v.Moeilijkheidsgraad,
                v.IsActief,
                v.Keuzes.Select(k => MeerkeuzeOptieMapper.MapNaarDomein(k)).ToList()
            );
        }

        public static List<VraagUI> MapLijstNaarUI(List<Vraag> vragen)
        {
            return vragen.Select(v => MapNaarUI(v)).ToList();
        }

        public static List<Vraag> MapLijstNaarDomein(List<VraagUI> vragen)
        {
            return vragen.Select(v => MapNaarDomein(v)).ToList();
        }
    }
}