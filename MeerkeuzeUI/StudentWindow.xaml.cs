using BusinessLaag.Managers; 
using BusinessLaag.Model;
using MeerkeuzeUI.ModelUI;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MeerkeuzeUI
{
    public partial class StudentWindow : Window
    {
        
        private Test _huidigeTest;
        private Student _huidigeStudent;
        private ScoreManager _scoreManager;

        public StudentWindow()
        {
            InitializeComponent();

            

            _scoreManager = new ScoreManager();
        }

        private void BtnStartTest_Click(object sender, RoutedEventArgs e)
        {
            string naam = TxtStudentNaam.Text.Trim();
            string code = TxtDeelnameCode.Text.Trim();

           
            if (string.IsNullOrWhiteSpace(naam) || string.IsNullOrWhiteSpace(code))
            {
                MessageBox.Show("Vul zowel je naam als de testcode in!", "Invoer vereist", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                

                
                _huidigeTest = new Test(1, code, DateTime.Now, new List<Vraag>(), new List<string>());
                _huidigeStudent = new Student(0, naam);

                if (_huidigeTest == null)
                {
                    MessageBox.Show("Deze testcode bestaat niet!", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                
                TxtWeergaveStudentNaam.Text = _huidigeStudent.Naam;
                TxtWeergaveOnderwerp.Text = $"Test: {_huidigeTest.Code}";

                
                ICVragen.ItemsSource = _huidigeTest.VragenInTest;

               
                BorderAanmelden.Visibility = Visibility.Collapsed;
                GridTestOmgeving.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Er liep iets mis bij het starten: {ex.Message}");
            }
        }

        private void BtnIndienen_Click(object sender, RoutedEventArgs e)
        {
            int behaaldeScore = 0;
            int totaalVragen = _huidigeTest.VragenInTest.Count;

            
            List<FeedbackResultaat> feedbackLijst = new List<FeedbackResultaat>();

            for (int i = 0; i < totaalVragen; i++)
            {
                Vraag huidigeVraag = _huidigeTest.VragenInTest[i];

                string gekozenTekst = "Geen antwoord ingevuld";
                string correcteTekst = "Onbekend";
                bool correctIngevuld = false;

                
                foreach (var optie in huidigeVraag.Keuzes)
                {
                    if (optie.IsCorrect) correcteTekst = optie.Tekst;
                }

                
                ContentPresenter container = ICVragen.ItemContainerGenerator.ContainerFromIndex(i) as ContentPresenter;
                if (container != null)
                {
                    ListBox lbOpties = ZoekVisualChild<ListBox>(container);
                    if (lbOpties != null && lbOpties.SelectedItem != null)
                    {
                        MeerkeuzeOptie gekozenOptie = lbOpties.SelectedItem as MeerkeuzeOptie;
                        gekozenTekst = gekozenOptie.Tekst;
                        correctIngevuld = gekozenOptie.IsCorrect;

                        if (correctIngevuld) behaaldeScore++;
                    }
                }

               
                feedbackLijst.Add(new FeedbackResultaat
                {
                    VraagInhoud = $"{i + 1}. {huidigeVraag.Inhoud}",
                    GekozenAntwoord = gekozenTekst,
                    CorrectAntwoord = correcteTekst,
                    IsCorrect = correctIngevuld
                });
            }

            
            TxtEindScoreWeergave.Text = $"{behaaldeScore} / {totaalVragen}";
            DGFeedback.ItemsSource = feedbackLijst;

            
            GridTestOmgeving.Visibility = Visibility.Collapsed;
            GridResultaatScherm.Visibility = Visibility.Visible;
        }

        private void BtnSluiten_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        
        private T ZoekVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is T o) return o;

                var childOfChild = ZoekVisualChild<T>(child);
                if (childOfChild != null) return childOfChild;
            }
            return null;
        }
    }
}