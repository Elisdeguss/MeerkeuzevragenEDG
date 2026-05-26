using BusinessLaag.Managers;

using MeerkeuzeUI.Mappers;

using MeerkeuzeUI.ModelUI;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace MeerkeuzeUI
{
    public partial class BewerkTestWindow : Window
    {
        private readonly TestManager _testManager;
        private readonly VraagManager _vraagManager;


        private readonly ObservableCollection<VraagUI> _testvragen;


        private ObservableCollection<VraagUI> _beschikbareVragen;

        public BewerkTestWindow(
            ObservableCollection<VraagUI> testvragen,
            TestManager testManager,
            VraagManager vraagManager)
        {
            InitializeComponent();

            _testvragen = testvragen;
            _testManager = testManager;
            _vraagManager = vraagManager;

            _beschikbareVragen = new ObservableCollection<VraagUI>();

            DGTestvragen.ItemsSource = _testvragen;


            DGBeschikbaar.ItemsSource = _beschikbareVragen;


            VulOnderwerpDropdown();
            VulMoeilijkheidsgraadDropdown();
        }



        private void VulOnderwerpDropdown()
        {
            try
            {
                var onderwerpen = _vraagManager.GetAlleOnderwerpen();
                CboOnderwerp.ItemsSource = onderwerpen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden onderwerpen: {ex.Message}");
            }
        }

        private void VulMoeilijkheidsgraadDropdown()
        {
            CboMoeilijkheid.ItemsSource = new[] { "Gemengd", "Beginner", "Expert", "Open level" };
            CboMoeilijkheid.SelectedIndex = 0;
        }



        private void BtnLaadVragen_Click(object sender, RoutedEventArgs e)
        {
            if (CboOnderwerp.SelectedItem == null)
            {
                MessageBox.Show("Kies eerst een onderwerp.", "Invoer vereist",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string onderwerp = CboOnderwerp.SelectedItem.ToString();
            string moeilijkheid = CboMoeilijkheid.SelectedItem?.ToString() ?? "Gemengd";

            try
            {
                
                var alleVragen = _vraagManager.GetVragenByOnderwerp(onderwerp, moeilijkheid);

                var testvraagIds = _testvragen.Select(v => v.Id).ToHashSet();
                var beschikbaar = alleVragen
                    .Where(v => !testvraagIds.Contains(v.Id))
                    .ToList();

                _beschikbareVragen.Clear();
                foreach (var v in VraagMapper.MapLijstNaarUI(beschikbaar))
                    _beschikbareVragen.Add(v);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden vragen: {ex.Message}",
                    "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void BtnVoegToe_Click(object sender, RoutedEventArgs e)
        {
            VoegGeselecteerdeVraagToe();
        }


        private void DGBeschikbaar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            VoegGeselecteerdeVraagToe();
        }

        private void VoegGeselecteerdeVraagToe()
        {
            if (DGBeschikbaar.SelectedItem is not VraagUI geselecteerde) return;


            _testvragen.Add(geselecteerde);


            _beschikbareVragen.Remove(geselecteerde);
        }


        private void BtnVerwijder_Click(object sender, RoutedEventArgs e)
        {
            VerwijderGeselecteerdeVraag();
        }


        private void DGTestvragen_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            VerwijderGeselecteerdeVraag();
        }

        private void VerwijderGeselecteerdeVraag()
        {
            if (DGTestvragen.SelectedItem is not VraagUI geselecteerde) return;

            _testvragen.Remove(geselecteerde);


            if (CboOnderwerp.SelectedItem?.ToString() == geselecteerde.Onderwerp)
                _beschikbareVragen.Add(geselecteerde);
        }



        private void BtnAccepteren_Click(object sender, RoutedEventArgs e)
        {
            var resultaat = MessageBox.Show(
                $"Test accepteren met {_testvragen.Count} vragen?\n\nJe keert terug naar het overzicht.",
                "Test bevestigen", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultaat == MessageBoxResult.Yes)
                this.Close();
        }

        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            var resultaat = MessageBox.Show(
                "Wil je de bewerkingen annuleren?\nWijzigingen worden niet ongedaan gemaakt.",
                "Annuleren", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (resultaat == MessageBoxResult.Yes)
                this.Close();
        }
    }
}