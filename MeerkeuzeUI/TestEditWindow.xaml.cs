using BusinessLaag.Managers;
using DataLaag.Mappers;
using DataLaag.ModelUI;
using MeerkeuzeUI.Mappers;
using MeerkeuzeUI.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MeerkeuzeUI
{
    public partial class BewerkTestWindow : Window
    {
        private readonly TestManager _testManager;
        private readonly VraagManager _vraagManager;

        // Dit is DEZELFDE lijst als in DocentenWindow — niet gekopieerd!
        // Wijzigingen hier zijn dus meteen zichtbaar in DocentenWindow.
        private readonly ObservableCollection<VraagUI> _testvragen;

        // Beschikbare vragen rechts in het scherm
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

            // Linker DataGrid — de huidige testvragen (gedeelde referentie!)
            DGTestvragen.ItemsSource = _testvragen;

            // Rechter DataGrid — beschikbare vragen om toe te voegen
            DGBeschikbaar.ItemsSource = _beschikbareVragen;

            // Dropdowns vullen
            VulOnderwerpDropdown();
            VulMoeilijkheidsgraadDropdown();
        }

        // ── Dropdowns ────────────────────────────────────────────────────────────

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

        // ── Beschikbare vragen laden ──────────────────────────────────────────────

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
                // Haal alle vragen op voor dit onderwerp
                var alleVragen = _testManager.GenereerTestVragen(
                    onderwerp, int.MaxValue, moeilijkheid);

                // Filter: toon enkel vragen die nog NIET in de test zitten
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

        // ── Vragen toevoegen aan de test ──────────────────────────────────────────

        // Via knop
        private void BtnVoegToe_Click(object sender, RoutedEventArgs e)
        {
            VoegGeselecteerdeVraagToe();
        }

        // Via dubbelklik op rechter DataGrid
        private void DGBeschikbaar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            VoegGeselecteerdeVraagToe();
        }

        private void VoegGeselecteerdeVraagToe()
        {
            if (DGBeschikbaar.SelectedItem is not VraagUI geselecteerde) return;

            // Voeg toe aan testvragen (ObservableCollection — DocentenWindow ziet dit meteen)
            _testvragen.Add(geselecteerde);

            // Verwijder uit beschikbare lijst zodat je hem niet dubbel kan toevoegen
            _beschikbareVragen.Remove(geselecteerde);
        }

        // ── Vragen verwijderen uit de test ───────────────────────────────────────

        // Via knop
        private void BtnVerwijder_Click(object sender, RoutedEventArgs e)
        {
            VerwijderGeselecteerdeVraag();
        }

        // Via dubbelklik op linker DataGrid
        private void DGTestvragen_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            VerwijderGeselecteerdeVraag();
        }

        private void VerwijderGeselecteerdeVraag()
        {
            if (DGTestvragen.SelectedItem is not VraagUI geselecteerde) return;

            _testvragen.Remove(geselecteerde);

            // Voeg terug toe aan beschikbare lijst als het onderwerp/niveau overeenkomt
            if (CboOnderwerp.SelectedItem?.ToString() == geselecteerde.Onderwerp)
                _beschikbareVragen.Add(geselecteerde);
        }

        // ── Accepteren / Annuleren ────────────────────────────────────────────────

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