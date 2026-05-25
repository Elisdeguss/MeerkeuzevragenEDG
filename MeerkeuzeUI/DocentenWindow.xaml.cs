using BusinessLaag.Interfaces;
using BusinessLaag.Managers;
using DataLaag;
using DataLaag.Mappers;
using DataLaag.ModelUI;
using DataLaag.Repositories;
using MeerkeuzeUI.Mappers;
using MeerkeuzeUI.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Path = System.IO.Path;

namespace MeerkeuzeUI
{
    public partial class DocentenWindow : Window
    {
        private readonly VraagManager _vraagManager;
        private readonly TestManager _testManager;

        // ObservableCollection — DataGrid update automatisch bij Add/Remove
        private ObservableCollection<VraagUI> _geselecteerdeVragen;

        public DocentenWindow()
        {
            InitializeComponent();

            // Config lezen en lagen opzetten
            string connectionString = LeesConnectionString();
            IVraagRepository vraagRepo = new VraagRepository(connectionString);
            ITestRepository testRepo = new TestRepository(connectionString);

            _vraagManager = new VraagManager(vraagRepo, new FileReaderFactory());
            _testManager = new TestManager(vraagRepo, testRepo);

            // ObservableCollection aanmaken en koppelen aan DataGrid via binding
            _geselecteerdeVragen = new ObservableCollection<VraagUI>();
            DGPreviewTestVragen.ItemsSource = _geselecteerdeVragen;

            // CollectionChanged: update teller automatisch als lijst verandert
            _geselecteerdeVragen.CollectionChanged += (s, e) =>
            {
                TxtTotaalVragenaantal.Text = _geselecteerdeVragen.Count.ToString();
            };

            SeedDatabaseVanuitResources();
            VulOnderwerpDropdowns();
            VulMoeilijkheidsgraadDropdowns();
        }

        // ── Configuratie ─────────────────────────────────────────────────────────

        private string LeesConnectionString()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            return config.GetConnectionString("SqlConnection");
        }

        // ── Dropdowns vullen ─────────────────────────────────────────────────────

        private void VulOnderwerpDropdowns()
        {
            try
            {
                // Via manager, nooit rechtstreeks via repository!
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

        // ── Data-import bij opstarten ─────────────────────────────────────────────

        private void SeedDatabaseVanuitResources()
        {
            try
            {
                string resourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
                if (!Directory.Exists(resourcesPath)) return;

                foreach (string bestand in Directory.GetFiles(resourcesPath, "*.txt"))
                {
                    try { _vraagManager.ImporteerBestand(bestand); }
                    catch { /* duplicaten worden stilletjes genegeerd */ }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij automatische data-seed: {ex.Message}",
                    "Waarschuwing", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // ── Tab: Test Genereren ───────────────────────────────────────────────────

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

                    // Businesslogica zit in de manager, niet hier!
                    var nieuweVragen = _testManager.GenereerTestVragen(
                        onderwerp, aantal, moeilijkheid,
                        VraagMapper.MapLijstNaarDomein(_geselecteerdeVragen.ToList()));

                    // Domein → UI via mapper, dan toevoegen aan ObservableCollection
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

            // We geven de ObservableCollection rechtstreeks mee —
            // wijzigingen in BewerkTestWindow zijn dus direct zichtbaar hier.
            var bewerkWindow = new BewerkTestWindow(_geselecteerdeVragen, _testManager, _vraagManager);
            bewerkWindow.ShowDialog();

            // Geen refresh nodig! ObservableCollection heeft de DataGrid al geüpdatet.
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

                // Vermijd overschrijven van bestaand bestand
                int teller = 1;
                while (File.Exists(doelBestand))
                    doelBestand = Path.Combine(downloadsPad, $"{basisNaam} ({teller++}).txt");

                // UI → domein via mapper, dan opslaan via manager
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

        // ── Moeilijkheidsgraad dynamisch updaten bij onderwerpkeuze ──────────────

        private void CboOnderwerp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ComboBox cboOnderwerp || cboOnderwerp.SelectedItem == null) return;

            // Zoek de bijhorende moeilijkheidsgraad-dropdown via de naam
            string suffix = cboOnderwerp.Name.Last().ToString();
            var cbos = new[] { CboMoeilijkheid1, CboMoeilijkheid2, CboMoeilijkheid3, CboMoeilijkheid4, CboMoeilijkheid5 };
            if (!int.TryParse(suffix, out int index) || index < 1 || index > cbos.Length) return;

            ComboBox cboMoeilijkheid = cbos[index - 1];
            string onderwerp = cboOnderwerp.SelectedItem.ToString();

            try
            {
                // Haal beschikbare moeilijkheidsgraden op via manager
                var vragen = _testManager.GenereerTestVragen(onderwerp, 1);
                var graden = vragen.Select(v => v.Moeilijkheidsgraad).Distinct().ToList();

                cboMoeilijkheid.ItemsSource = new[] { "Gemengd" }.Concat(graden).ToList();
                cboMoeilijkheid.SelectedIndex = 0;
            }
            catch
            {
                cboMoeilijkheid.ItemsSource = new[] { "Gemengd" };
                cboMoeilijkheid.SelectedIndex = 0;
            }
        }
    }
}