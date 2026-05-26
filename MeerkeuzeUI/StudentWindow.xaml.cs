using BusinessLaag.Interfaces;
using BusinessLaag.Managers;
using BusinessLaag.Model;
using DataLaag.Repositories;
using MeerkeuzeUI.Mappers;
using MeerkeuzeUI.ModelUI;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Windows;

namespace MeerkeuzeUI
{
    public partial class StudentWindow : Window
    {
        private readonly TestManager _testManager;
        private readonly ScoreManager _scoreManager;

        private TestUI _huidigeTest;
        private string _studentNaam;

        public StudentWindow()
        {
            InitializeComponent();

            string connectionString = LeesConnectionString();
            IVraagRepository vraagRepo = new VraagRepository(connectionString);
            ITestRepository testRepo = new TestRepository(connectionString);
            IScoreRepository scoreRepo = new ScoreRepository(connectionString);

            _testManager = new TestManager(vraagRepo, testRepo);
            _scoreManager = new ScoreManager(scoreRepo);
        }

        private string LeesConnectionString()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            return config.GetConnectionString("SqlConnection");
        }

        

        private void BtnStartTest_Click(object sender, RoutedEventArgs e)
        {
            string naam = TxtStudentNaam.Text.Trim();
            string code = TxtDeelnameCode.Text.Trim();

            if (string.IsNullOrWhiteSpace(naam) || string.IsNullOrWhiteSpace(code))
            {
                MessageBox.Show("Vul zowel je naam als de testcode in!",
                    "Invoer vereist", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                
                Test testDomein = _testManager.GetTestByCode(code);

                if (testDomein == null)
                {
                    MessageBox.Show($"Testcode '{code}' bestaat niet.",
                        "Niet gevonden", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _huidigeTest = TestMapper.MapNaarUI(testDomein);
                _studentNaam = naam;

                TxtWeergaveStudentNaam.Text = _studentNaam;
                TxtWeergaveOnderwerp.Text = $"Test: {_huidigeTest.Code}";

                
                ICVragen.ItemsSource = _huidigeTest.Vragen;

                BorderAanmelden.Visibility = Visibility.Collapsed;
                GridTestOmgeving.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het starten van de test:\n{ex.Message}",
                    "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       

        private void BtnIndienen_Click(object sender, RoutedEventArgs e)
        {

            var onbeantwoord = _huidigeTest.Vragen
                .Where(v => !v.Keuzes.Any(k => k.IsGeselecteerd))
                .ToList();

            if (onbeantwoord.Count > 0)
            {
                var keuze = MessageBox.Show(
                    $"{onbeantwoord.Count} vraag/vragen zijn nog niet beantwoord. Toch indienen?",
                    "Onvolledige test", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (keuze == MessageBoxResult.No) return;
            }

            try
            {
                int behaaldeScore = 0;
                int totaal = _huidigeTest.Vragen.Count;
                var feedbackLijst = new List<FeedbackResultaatUI>();

                foreach (var vraag in _huidigeTest.Vragen)
                {

                    var gekozenOptie = vraag.Keuzes.FirstOrDefault(k => k.IsGeselecteerd);
                    var correcteOptie = vraag.Keuzes.FirstOrDefault(k => k.IsCorrect);

                    bool isCorrect = gekozenOptie != null && gekozenOptie.IsCorrect;
                    if (isCorrect) behaaldeScore++;

                    feedbackLijst.Add(new FeedbackResultaatUI
                    {
                        VraagInhoud = vraag.Inhoud,
                        GekozenAntwoord = gekozenOptie?.Tekst ?? "Geen antwoord",
                        CorrectAntwoord = correcteOptie?.Tekst ?? "Onbekend",
                        IsCorrect = isCorrect
                    });
                }

                
                TxtEindScoreWeergave.Text = $"{behaaldeScore} / {totaal}";
                DGFeedback.ItemsSource = feedbackLijst;

                _scoreManager.SlaScoreOp(_studentNaam, _huidigeTest.Id, behaaldeScore);

                GridTestOmgeving.Visibility = Visibility.Collapsed;
                GridResultaatScherm.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het indienen:\n{ex.Message}",
                    "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            
            
        }

        private void BtnSluiten_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}