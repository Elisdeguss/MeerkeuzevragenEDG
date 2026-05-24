using BusinessLaag.Interfaces;
using BusinessLaag.Managers;
using BusinessLaag.Model;
using DataLaag;
using DataLaag.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace MeerkeuzeUI
{
    /// <summary>
    /// Interaction logic for DocentenWindow.xaml
    /// </summary>
    public partial class DocentenWindow : Window
    {

        private string connectionString;

        private readonly VraagManager _vraagManager;
        private readonly TestManager _testManager;
        private readonly IVraagRepository _vraagRepo;


        private ObservableCollection<Vraag> _geselecteerdeVragen;
        public DocentenWindow()
        {
            InitializeComponent();

            LeesConfig();

            _vraagRepo = new VraagRepository(connectionString);
            _vraagManager = new VraagManager(_vraagRepo, new FileReaderFactory());
            _testManager = new TestManager(_vraagRepo);

            _geselecteerdeVragen = new ObservableCollection<Vraag>();


            DGPreviewTestVragen.ItemsSource = _geselecteerdeVragen;


            _geselecteerdeVragen.CollectionChanged += GeselecteerdeVragen_CollectionChanged;


            SeedDatabaseVanuitResources();
            VulMoeilijkheidsgraadDropdowns();
            VulOnderwerpDropdowns();
        }

        private void GeselecteerdeVragen_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            TxtTotaalVragenaantal.Text = _geselecteerdeVragen.Count.ToString();
        }

        private void LeesConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var config = builder.Build();

            connectionString = config.GetConnectionString("SqlConnection");
        }

        private void SeedDatabaseVanuitResources()
        {
            try
            {
                string resourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");

                if (Directory.Exists(resourcesPath))
                {
                    string[] txtBestanden = Directory.GetFiles(resourcesPath, "*.txt");

                    foreach (string bestand in txtBestanden)
                    {
                        try
                        {

                            _vraagManager.ImporteerBestand(bestand);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij automatische data-seed: {ex.Message}", "Waarschuwing", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void VulMoeilijkheidsgraadDropdowns()
        {
            List<string> opties = new List<string> { "Gemengd", "Beginner", "Expert", "Open level" };

            foreach (string optie in opties)
            {
                CboMoeilijkheid1.Items.Add(optie);
                CboMoeilijkheid2.Items.Add(optie);
                CboMoeilijkheid3.Items.Add(optie);
                CboMoeilijkheid4.Items.Add(optie);
                CboMoeilijkheid5.Items.Add(optie);
            }

            CboMoeilijkheid1.SelectedIndex = 0;
            CboMoeilijkheid2.SelectedIndex = 0;
            CboMoeilijkheid3.SelectedIndex = 0;
            CboMoeilijkheid4.SelectedIndex = 0;
            CboMoeilijkheid5.SelectedIndex = 0;
        }

        private void VulOnderwerpDropdowns()
        {
            try
            {
                List<string> onderwerpen = _vraagRepo.GetAlleOnderwerpen();

                foreach (string onderwerp in onderwerpen)
                {
                    CboOnderwerp1.Items.Add(onderwerp);
                    CboOnderwerp2.Items.Add(onderwerp);
                    CboOnderwerp3.Items.Add(onderwerp);
                    CboOnderwerp4.Items.Add(onderwerp);
                    CboOnderwerp5.Items.Add(onderwerp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het inladen van de onderwerpen: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGenereerTest_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtTestCode.Text))
            {
                MessageBox.Show("Vul eerst een unieke testcode in!", "Invoer vereist", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _geselecteerdeVragen.Clear();


            var invoerRijen = new[]
            {
        new { OnderwerpCbo = CboOnderwerp1, MoeilijkheidCbo = CboMoeilijkheid1, AantalTxt = TxtAantal1 },
        new { OnderwerpCbo = CboOnderwerp2, MoeilijkheidCbo = CboMoeilijkheid2, AantalTxt = TxtAantal2 },
        new { OnderwerpCbo = CboOnderwerp3, MoeilijkheidCbo = CboMoeilijkheid3, AantalTxt = TxtAantal3 },
        new { OnderwerpCbo = CboOnderwerp4, MoeilijkheidCbo = CboMoeilijkheid4, AantalTxt = TxtAantal4 },
        new { OnderwerpCbo = CboOnderwerp5, MoeilijkheidCbo = CboMoeilijkheid5, AantalTxt = TxtAantal5 }
    };

            Random rng = new Random();

            try
            {
                foreach (var rij in invoerRijen)
                {

                    if (rij.OnderwerpCbo.SelectedItem == null) continue;

                    string onderwerp = rij.OnderwerpCbo.SelectedItem.ToString();
                    string moeilijkheid = rij.MoeilijkheidCbo.SelectedItem?.ToString() ?? "Gemengd";


                    if (!int.TryParse(rij.AantalTxt.Text, out int gevraagdAantal) || gevraagdAantal <= 0) continue;


                    List<Vraag> alleVragen = _vraagRepo.GetVragenByOnderwerp(onderwerp, true);


                    List<Vraag> niveauGefilterd = new List<Vraag>();
                    foreach (Vraag v in alleVragen)
                    {
                        if (moeilijkheid == "Gemengd" || v.Moeilijkheidsgraad == moeilijkheid)
                        {
                            niveauGefilterd.Add(v);
                        }
                    }


                    List<Vraag> beschikbareUniekeVragen = new List<Vraag>();
                    foreach (Vraag v in niveauGefilterd)
                    {
                        bool zitAlInTest = false;
                        foreach (Vraag reedsGekozen in _geselecteerdeVragen)
                        {
                            if (reedsGekozen.Id == v.Id)
                            {
                                zitAlInTest = true;
                                break;
                            }
                        }


                        if (!zitAlInTest)
                        {
                            beschikbareUniekeVragen.Add(v);
                        }
                    }


                    if (beschikbareUniekeVragen.Count < gevraagdAantal)
                    {
                        MessageBox.Show($"Niet genoeg unieke vragen voor {onderwerp} ({moeilijkheid}).\n" +
                                        $"Je vraagt er {gevraagdAantal}, maar er zijn er nog maar {beschikbareUniekeVragen.Count} over.",
                                        "Te weinig vragen", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }


                    for (int i = beschikbareUniekeVragen.Count - 1; i > 0; i--)
                    {
                        int j = rng.Next(i + 1);
                        Vraag temp = beschikbareUniekeVragen[i];
                        beschikbareUniekeVragen[i] = beschikbareUniekeVragen[j];
                        beschikbareUniekeVragen[j] = temp;
                    }


                    for (int i = 0; i < gevraagdAantal; i++)
                    {
                        _geselecteerdeVragen.Add(beschikbareUniekeVragen[i]);
                    }
                }

                if (_geselecteerdeVragen.Count == 0)
                {
                    MessageBox.Show("Vul minstens één rij correct in om een test te kunnen genereren.", "Informatie", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het genereren: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnOpslaanEnExporteren_Click(object sender, RoutedEventArgs e)
        {
            string testCode = TxtTestCode.Text.Trim();
            if (string.IsNullOrWhiteSpace(testCode))
            {
                MessageBox.Show("Vul een unieke testcode in voordat je de test opslaat!", "Invoer Vereist", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            if (_geselecteerdeVragen == null || _geselecteerdeVragen.Count == 0)
            {
                MessageBox.Show("Er zijn nog geen vragen gegenereerd om op te slaan. Klik eerst op 'Genereer Test'.", "Geen Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {

                string downloadsPad = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

                string basisNaam = $"Test_{testCode}";
                string extensie = ".txt";
                string doelBestand = Path.Combine(downloadsPad, basisNaam + extensie);


                int teller = 1;
                while (File.Exists(doelBestand))
                {
                    doelBestand = Path.Combine(downloadsPad, $"{basisNaam} ({teller}){extensie}");
                    teller++;
                }


                List<Vraag> vragenLijst = new List<Vraag>(_geselecteerdeVragen);


                _vraagManager.OpslaanEnExporteerTest(testCode, vragenLijst, doelBestand);


                MessageBox.Show($"De test '{testCode}' is succesvol opgeslagen in de database en geëxporteerd naar je Downloads-map!",
                                "Succesvol Geëxporteerd", MessageBoxButton.OK, MessageBoxImage.Information);


                System.Diagnostics.Process.Start("explorer.exe", downloadsPad);


                TxtTestCode.Clear();
                _geselecteerdeVragen.Clear();
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Er is een fout opgetreden bij het verwerken van de test:\n{ex.Message}",
                                "Fout bij opslaan", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }
        private void CboOnderwerp_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboBox cboOnderwerp = sender as ComboBox;
            if (cboOnderwerp == null || cboOnderwerp.SelectedItem == null) return;

            string gekozenOnderwerp = cboOnderwerp.SelectedItem.ToString();


            ComboBox cboMoeilijkheid = null;
            if (cboOnderwerp.Name == "CboOnderwerp1") cboMoeilijkheid = CboMoeilijkheid1;
            else if (cboOnderwerp.Name == "CboOnderwerp2") cboMoeilijkheid = CboMoeilijkheid2;
            else if (cboOnderwerp.Name == "CboOnderwerp3") cboMoeilijkheid = CboMoeilijkheid3;
            else if (cboOnderwerp.Name == "CboOnderwerp4") cboMoeilijkheid = CboMoeilijkheid4;
            else if (cboOnderwerp.Name == "CboOnderwerp5") cboMoeilijkheid = CboMoeilijkheid5;

            if (cboMoeilijkheid == null) return;


            cboMoeilijkheid.Items.Clear();
            cboMoeilijkheid.Items.Add("Gemengd");

            try
            {

                List<Vraag> vragen = _vraagRepo.GetVragenByOnderwerp(gekozenOnderwerp, true);


                List<string> gevondenGraden = new List<string>();
                foreach (Vraag v in vragen)
                {
                    if (!gevondenGraden.Contains(v.Moeilijkheidsgraad))
                    {
                        gevondenGraden.Add(v.Moeilijkheidsgraad);
                    }
                }


                foreach (string graad in gevondenGraden)
                {
                    cboMoeilijkheid.Items.Add(graad);
                }


                cboMoeilijkheid.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het ophalen van moeilijkheidsgraden: {ex.Message}");
            }
        }
    }
}
