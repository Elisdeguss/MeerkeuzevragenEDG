using BusinessLaag.Interfaces;
using BusinessLaag.Managers;
using BusinessLaag.Model;
using DataLaag;
using DataLaag.Repositories;
using MeerkeuzeUI.Mappers;
using MeerkeuzeUI.ModelUI;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Path = System.IO.Path;

namespace MeerkeuzeUI
{
    public partial class DocentenWindow : Window
    {
        private readonly VraagManager _vraagManager;
        private readonly TestManager _testManager;
        private readonly ScoreManager _scoreManager;


        private ObservableCollection<VraagUI> _geselecteerdeVragen;

        public DocentenWindow()
        {
            InitializeComponent();


            string connectionString = LeesConnectionString();
            IVraagRepository vraagRepo = new VraagRepository(connectionString);
            ITestRepository testRepo = new TestRepository(connectionString);
            IScoreRepository scoreRepo = new ScoreRepository(connectionString);

            _vraagManager = new VraagManager(vraagRepo, new FileReaderFactory());
            _testManager = new TestManager(vraagRepo, testRepo);
            _scoreManager = new ScoreManager(scoreRepo);

            _geselecteerdeVragen = new ObservableCollection<VraagUI>();
            DGPreviewTestVragen.ItemsSource = _geselecteerdeVragen;


            _geselecteerdeVragen.CollectionChanged += (s, e) =>
            {
                TxtTotaalVragenaantal.Text = _geselecteerdeVragen.Count.ToString();
            };

            SeedDatabaseVanuitResources();
            VulOnderwerpDropdowns();
            VulMoeilijkheidsgraadDropdowns();
            VulBestaandeTestsDropdown();
        }



        private string LeesConnectionString()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            return config.GetConnectionString("SqlConnection");
        }



        private void VulOnderwerpDropdowns()
        {
            try
            {

                var onderwerpen = _vraagManager.GetAlleOnderwerpen();

                foreach (var cbo in new[] { CboOnderwerp1, CboOnderwerp2, CboOnderwerp3, CboOnderwerp4, CboOnderwerp5 })
                {
                    cbo.ItemsSource = onderwerpen;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het inladen van de onderwerpen: {ex.Message}",
                    "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void VulMoeilijkheidsgraadDropdowns()
        {
            var opties = new[] { "Gemengd", "Beginner", "Expert", "Open level" };

            foreach (var cbo in new[] { CboMoeilijkheid1, CboMoeilijkheid2, CboMoeilijkheid3, CboMoeilijkheid4, CboMoeilijkheid5 })
            {
                cbo.ItemsSource = opties;
                cbo.SelectedIndex = 0;
            }
        }



        private void SeedDatabaseVanuitResources()
        {
            try
            {
                string resourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
                if (!Directory.Exists(resourcesPath)) return;

                foreach (string bestand in Directory.GetFiles(resourcesPath, "*.txt"))
                {
                    try { _vraagManager.ImporteerBestand(bestand); }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij automatische data-seed: {ex.Message}",
                    "Waarschuwing", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }



        private void BtnGenereerTest_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtTestCode.Text))
            {
                MessageBox.Show("Vul eerst een unieke testcode in!",
                    "Invoer vereist", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _geselecteerdeVragen.Clear();

            var invoerRijen = new[]
            {
                new { Onderwerp = CboOnderwerp1, Moeilijkheid = CboMoeilijkheid1, Aantal = TxtAantal1 },
                new { Onderwerp = CboOnderwerp2, Moeilijkheid = CboMoeilijkheid2, Aantal = TxtAantal2 },
                new { Onderwerp = CboOnderwerp3, Moeilijkheid = CboMoeilijkheid3, Aantal = TxtAantal3 },
                new { Onderwerp = CboOnderwerp4, Moeilijkheid = CboMoeilijkheid4, Aantal = TxtAantal4 },
                new { Onderwerp = CboOnderwerp5, Moeilijkheid = CboMoeilijkheid5, Aantal = TxtAantal5 },
            };

            try
            {
                foreach (var rij in invoerRijen)
                {
                    if (rij.Onderwerp.SelectedItem == null) continue;
                    if (!int.TryParse(rij.Aantal.Text, out int aantal) || aantal <= 0) continue;

                    string onderwerp = rij.Onderwerp.SelectedItem.ToString();
                    string moeilijkheid = rij.Moeilijkheid.SelectedItem?.ToString() ?? "Gemengd";


                    var nieuweVragen = _testManager.GenereerTestVragen(
                        onderwerp, aantal, moeilijkheid,
                        VraagMapper.MapLijstNaarDomein(_geselecteerdeVragen.ToList()));


                    foreach (var v in VraagMapper.MapLijstNaarUI(nieuweVragen))
                        _geselecteerdeVragen.Add(v);
                }

                if (_geselecteerdeVragen.Count == 0)
                    MessageBox.Show("Vul minstens één rij correct in.",
                        "Informatie", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het genereren: {ex.Message}",
                    "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBewerkTest_Click(object sender, RoutedEventArgs e)
        {
            if (_geselecteerdeVragen.Count == 0)
            {
                MessageBox.Show("Genereer eerst een test voordat je die manueel bewerkt.",
                    "Geen vragen", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            var bewerkWindow = new BewerkTestWindow(_geselecteerdeVragen, _testManager, _vraagManager);
            bewerkWindow.ShowDialog();


        }

        private void BtnOpslaanEnExporteren_Click(object sender, RoutedEventArgs e)
        {
            string testCode = TxtTestCode.Text.Trim();

            if (string.IsNullOrWhiteSpace(testCode))
            {
                MessageBox.Show("Vul een unieke testcode in voordat je de test opslaat!",
                    "Invoer vereist", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_geselecteerdeVragen.Count == 0)
            {
                MessageBox.Show("Genereer eerst een test voor je opslaat.",
                    "Geen data", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string downloadsPad = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                string basisNaam = $"Test_{testCode}";
                string doelBestand = Path.Combine(downloadsPad, $"{basisNaam}.txt");


                int teller = 1;
                while (File.Exists(doelBestand))
                    doelBestand = Path.Combine(downloadsPad, $"{basisNaam} ({teller++}).txt");


                var vragenDomein = VraagMapper.MapLijstNaarDomein(_geselecteerdeVragen.ToList());
                _testManager.OpslaanEnExporteerTest(testCode, vragenDomein, doelBestand);

                MessageBox.Show($"Test '{testCode}' opgeslagen en geëxporteerd naar je Downloads-map!",
                    "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                System.Diagnostics.Process.Start("explorer.exe", downloadsPad);

                TxtTestCode.Clear();
                _geselecteerdeVragen.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het opslaan:\n{ex.Message}",
                    "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void CboOnderwerp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ComboBox cboOnderwerp || cboOnderwerp.SelectedItem == null) return;

            string suffix = cboOnderwerp.Name.Last().ToString();
            var cbos = new[] { CboMoeilijkheid1, CboMoeilijkheid2, CboMoeilijkheid3, CboMoeilijkheid4, CboMoeilijkheid5 };
            if (!int.TryParse(suffix, out int index) || index < 1 || index > cbos.Length) return;

            ComboBox cboMoeilijkheid = cbos[index - 1];
            string onderwerp = cboOnderwerp.SelectedItem.ToString();

            try
            {

                var alleVragen = _vraagManager.GetVragenByOnderwerp(onderwerp);
                var niveaus = alleVragen
                    .Select(v => v.Moeilijkheidsgraad)
                    .Distinct()
                    .OrderBy(n => n)
                    .ToList();

                cboMoeilijkheid.ItemsSource = new[] { "Gemengd" }.Concat(niveaus).ToList();
                cboMoeilijkheid.SelectedIndex = 0;
            }
            catch
            {
                cboMoeilijkheid.ItemsSource = new[] { "Gemengd" };
                cboMoeilijkheid.SelectedIndex = 0;
            }
        }
        private class TestVraagRijUI
        {
            public int Nummer { get; set; }
            public string Inhoud { get; set; }
            public string JuistAntwoord { get; set; }
            public string Moeilijkheidsgraad { get; set; }
        }

        
        private class TestGemiddeldeRijUI
        {
            public string TestCode { get; set; }
            public int AantalDeelnemers { get; set; }
            public double GemiddeldeOp20 { get; set; }
            public string GemiddeldeOp20Weergave => $"{GemiddeldeOp20} / 20";
        }


        private void VulBestaandeTestsDropdown()
        {
            try
            {
                var testen = _testManager.GetAlleTests();

                CboBestaandeTests.ItemsSource = testen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden tests: {ex.Message}",
                    "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CboBestaandeTests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboBestaandeTests.SelectedItem is not Test geselecteerdeTest) return;

            try
            {
                
                var test = _testManager.GetTestByCode(geselecteerdeTest.Code);

                
                var rijen = test.VragenInTest
                    .Select((v, index) => new TestVraagRijUI
                    {
                        Nummer = index + 1,
                        Inhoud = v.Inhoud,
                        
                        JuistAntwoord = v.Keuzes.FirstOrDefault(k => k.IsCorrect)?.Tekst ?? "?",
                        Moeilijkheidsgraad = v.Moeilijkheidsgraad
                    })
                    .ToList();

                DGBestaandeTestVragen.ItemsSource = rijen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden test: {ex.Message}",
                    "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (sender is TabControl tc && tc.SelectedIndex == 2)
                LaadScores();
        }

        private void LaadScores()
        {
            try
            {
                var alleScores = _scoreManager.GetAlleScores();

                
                var perStudent = alleScores
                    .GroupBy(r => r.Student.Naam)
                    .SelectMany(g =>
                    {
                        int totaalPunten = g.Sum(r => r.Score);
                        int totaalVragen = g.Sum(r => r.TotaalVragen);

                        return g.Select(r => new ScoreRijUI
                        {
                            StudentNaam = r.Student.Naam,
                            TestCode = r.Test.Code,
                            BehaaldePunten = r.Score,
                            TotaalVragen = r.TotaalVragen,
                            TotaalPuntenAlleTests = totaalPunten,   
                            TotaalVragenAlleTests = totaalVragen   
                        });
                    })
                    .ToList();

                DGScoresPerStudent.ItemsSource = perStudent;

                
                var perTest = alleScores
                    .GroupBy(r => r.Test.Code)
                    .Select(g => new TestGemiddeldeRijUI
                    {
                        TestCode = g.Key,
                        AantalDeelnemers = g.Count(),
                        GemiddeldeOp20 = g.Sum(r => r.TotaalVragen) == 0 ? 0 :
                            Math.Round(
                                g.Average(r => r.TotaalVragen == 0 ? 0 :
                                    (double)r.Score / r.TotaalVragen * 20), 1)
                    })
                    .ToList();

                DGGemiddeldePerTest.ItemsSource = perTest;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden scores: {ex.Message}",
                    "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnVragenBeheren_Click(object sender, RoutedEventArgs e)
        {
            var beheerWindow = new VragenBeheerWindow(_vraagManager);
            beheerWindow.ShowDialog();

            
            VulOnderwerpDropdowns();
            VulBestaandeTestsDropdown();
        }
    }

}