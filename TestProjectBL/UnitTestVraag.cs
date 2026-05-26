using BusinessLaag.Exceptions;
using BusinessLaag.Model;

namespace TestProjectBL
{
    public class UnitTestVraag
    {
        
        private List<MeerkeuzeOptie> MaakGeldigeKeuzes()
        {
            return new List<MeerkeuzeOptie>
            {
                new MeerkeuzeOptie(1, "Structured Query Language", true),
                new MeerkeuzeOptie(2, "Simple Query Language", false),
                new MeerkeuzeOptie(3, "Sequential Query Language", false),
                new MeerkeuzeOptie(4, "System Query Logic", false)
            };
        }

        

        [Fact]
        public void Constructor_GeldigeWaarden_MaaktVraagAan()
        {
            
            var keuzes = MaakGeldigeKeuzes();

            
            var vraag = new Vraag(1, "SQL", "Waar staat SQL voor?", "Beginner", true, keuzes);

            
            Assert.Equal(1, vraag.Id);
            Assert.Equal("SQL", vraag.Onderwerp);
            Assert.Equal("Waar staat SQL voor?", vraag.Inhoud);
            Assert.Equal("Beginner", vraag.Moeilijkheidsgraad);
            Assert.True(vraag.IsActief);
            Assert.Equal(4, vraag.Keuzes.Count);
        }

        [Theory]
        [InlineData("", "Waar staat SQL voor?", "Beginner")]
        [InlineData(null, "Waar staat SQL voor?", "Beginner")]
        [InlineData("  ", "Waar staat SQL voor?", "Beginner")]
        [InlineData(" SQL", "Waar staat SQL voor?", "Beginner")]
        [InlineData("SQL ", "Waar staat SQL voor?", "Beginner")]
        [InlineData("SQL", "", "Beginner")]
        [InlineData("SQL", null, "Beginner")]
        [InlineData("SQL", "  ", "Beginner")]
        [InlineData("SQL", "Waar staat SQL voor?", "")]
        [InlineData("SQL", "Waar staat SQL voor?", null)]
        public void Constructor_OngeldigeWaarden_GooidException(string onderwerp, string inhoud, string moeilijkheid)
        {
            
            var keuzes = MaakGeldigeKeuzes();

            
            Assert.Throws<VraagExceptions>(() =>
                new Vraag(1, onderwerp, inhoud, moeilijkheid, true, keuzes));
        }

        

        [Theory]
        [InlineData("Waar staat SQL voor?")]
        [InlineData("Welke clausule gebruik je om te filteren?")]
        public void Inhoud_GeldigeWaarde_WordtOpgeslagen(string inhoud)
        {
            
            var vraag = new Vraag(1, "SQL", "Oorspronkelijke vraag?", "Beginner", true, MaakGeldigeKeuzes());

            
            vraag.Inhoud = inhoud;

            
            Assert.Equal(inhoud, vraag.Inhoud);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData(" Vraag met spatie voor")]
        [InlineData("Vraag met spatie achter ")]
        public void Inhoud_OngeldigeWaarde_GooidException(string inhoud)
        {
            
            var vraag = new Vraag(1, "SQL", "Oorspronkelijke vraag?", "Beginner", true, MaakGeldigeKeuzes());

            Assert.Throws<VraagExceptions>(() => vraag.Inhoud = inhoud);
        }


        [Theory]
        [InlineData("SQL")]
        [InlineData("Muziek80s")]
        public void Onderwerp_GeldigeWaarde_WordtOpgeslagen(string onderwerp)
        {
            
            var vraag = new Vraag(1, "SQL", "Vraag?", "Beginner", true, MaakGeldigeKeuzes());

            vraag.Onderwerp = onderwerp;

            Assert.Equal(onderwerp, vraag.Onderwerp);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("  ")]
        [InlineData(" SQL")]
        [InlineData("SQL ")]
        public void Onderwerp_OngeldigeWaarde_GooidException(string onderwerp)
        {
            var vraag = new Vraag(1, "SQL", "Vraag?", "Beginner", true, MaakGeldigeKeuzes());

            Assert.Throws<VraagExceptions>(() => vraag.Onderwerp = onderwerp);
        }


        [Fact]
        public void Keuzes_MinstesTweeOpties_WordtOpgeslagen()
        {
            var vraag = new Vraag(1, "SQL", "Vraag?", "Beginner", true, MaakGeldigeKeuzes());
            var nieuweKeuzes = new List<MeerkeuzeOptie>
            {
                new MeerkeuzeOptie(1, "Optie A", true),
                new MeerkeuzeOptie(2, "Optie B", false)
            };

            vraag.Keuzes = nieuweKeuzes;

            Assert.Equal(2, vraag.Keuzes.Count);
        }

        [Fact]
        public void Keuzes_MaarEenOptie_GooidException()
        {
            var vraag = new Vraag(1, "SQL", "Vraag?", "Beginner", true, MaakGeldigeKeuzes());
            var teWeinig = new List<MeerkeuzeOptie>
            {
                new MeerkeuzeOptie(1, "Enige optie", true)
            };

            Assert.Throws<VraagExceptions>(() => vraag.Keuzes = teWeinig);
        }

        [Fact]
        public void Keuzes_Null_GooidException()
        {
            var vraag = new Vraag(1, "SQL", "Vraag?", "Beginner", true, MaakGeldigeKeuzes());

            Assert.Throws<VraagExceptions>(() => vraag.Keuzes = null);
        }

        [Fact]
        public void Keuzes_GeenCorrectAntwoord_GooidException()
        {
            var vraag = new Vraag(1, "SQL", "Vraag?", "Beginner", true, MaakGeldigeKeuzes());
            var geenCorrect = new List<MeerkeuzeOptie>
            {
                new MeerkeuzeOptie(1, "Optie A", false),
                new MeerkeuzeOptie(2, "Optie B", false),
                new MeerkeuzeOptie(3, "Optie C", false)
            };

            Assert.Throws<VraagExceptions>(() => vraag.Keuzes = geenCorrect);
        }

        [Fact]
        public void Keuzes_OngeldigeWaarde_PasstBestaandeKeuzeNietAan()
        {
            var vraag = new Vraag(1, "SQL", "Vraag?", "Beginner", true, MaakGeldigeKeuzes());
            int aantalVoor = vraag.Keuzes.Count;

            try { vraag.Keuzes = null; } catch { }

            Assert.Equal(aantalVoor, vraag.Keuzes.Count);
        }
    }
}