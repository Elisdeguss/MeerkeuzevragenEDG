using BusinessLaag.Model;

using MeerkeuzeUI.ModelUI;

namespace MeerkeuzeUI.Mappers
{
    public static class TestMapper
    {

        public static TestUI MapNaarUI(Test t)
        {
            return new TestUI(
                t.Id,
                t.Code,
                t.AanmaakDatum,
                t.VragenInTest.Select(v => VraagMapper.MapNaarUI(v)).ToList(),
                t.Onderwerpen
            );
        }


        public static Test MapNaarDomein(TestUI t)
        {
            return new Test(
                t.Id,
                t.Code,
                t.AanmaakDatum,
                t.Vragen.Select(v => VraagMapper.MapNaarDomein(v)).ToList(),
                t.Onderwerpen
            );
        }


        public static List<TestUI> MapLijstNaarUI(List<Test> testen)
        {
            return testen.Select(t => MapNaarUI(t)).ToList();
        }
    }
}
