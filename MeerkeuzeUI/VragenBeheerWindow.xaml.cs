using BusinessLaag.Exceptions;
using BusinessLaag.Managers;
using BusinessLaag.Model;
using MeerkeuzeUI.Mappers;
using MeerkeuzeUI.ModelUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MeerkeuzeUI
{
    public partial class VragenBeheerWindow : Window
    {
        private readonly VraagManager _vraagManager;

        
        private ObservableCollection<VraagUI> _actieveVragen;

        public VragenBeheerWindow(VraagManager vraagManager)
        {
            InitializeComponent();

            _vraagManager = vraagManager;
            _actieveVragen = new ObservableCollection<VraagUI>();
            DGActieveVragen.ItemsSource = _actieveVragen;

            VulDropdowns();
        }

        

        private void VulDropdowns()
        {
            try
            {
                var onderwerpen = _vraagManager.GetAlleOnderwerpen();

                
                CboOnderwerpBestaand.ItemsSource = onderwerpen;

                
                var filterLijst = new List<string> { "Alle onderwerpen" };
                filterLijst.AddRange(onderwerpen);
                CboFilterOnderwerp.ItemsSource = filterLijst;
                CboFilterOnderwerp.SelectedIndex = 0;

               
                CboMoeilijkheid.ItemsSource = new[] { "Beginner", "Expert", "Open level" };
                CboMoeilijkheid.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden gegevens: {ex.Message}",
                    "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            LaadActieveVragen();
        }

        
        private void CboOnderwerpBestaand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboOnderwerpBestaand.SelectedItem != null)
                TxtOnderwerpNieuw.Clear();
        }

        private void TxtOnderwerpNieuw_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtOnderwerpNieuw.Text))
                CboOnderwerpBestaand.SelectedIndex = -1;
        }

        private void BtnSlaVraagOp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string onderwerp = !string.IsNullOrWhiteSpace(TxtOnderwerpNieuw.Text)
                    ? TxtOnderwerpNieuw.Text.Trim()
                    : CboOnderwerpBestaand.SelectedItem?.ToString();

                string correcteOptie = null;
                if (RbCorrectA.IsChecked == true) correcteOptie = TxtOptieA.Text.Trim();
                else if (RbCorrectB.IsChecked == true) correcteOptie = TxtOptieB.Text.Trim();
                else if (RbCorrectC.IsChecked == true) correcteOptie = TxtOptieC.Text.Trim();
                else if (RbCorrectD.IsChecked == true) correcteOptie = TxtOptieD.Text.Trim();

                var opties = new List<MeerkeuzeOptie>
                {
                    new MeerkeuzeOptie(0, TxtOptieA.Text.Trim(), correcteOptie == TxtOptieA.Text.Trim()),
                    new MeerkeuzeOptie(0, TxtOptieB.Text.Trim(), correcteOptie == TxtOptieB.Text.Trim()),
                    new MeerkeuzeOptie(0, TxtOptieC.Text.Trim(), correcteOptie == TxtOptieC.Text.Trim()),
                    new MeerkeuzeOptie(0, TxtOptieD.Text.Trim(), correcteOptie == TxtOptieD.Text.Trim()),
                };

                
                var nieuweVraag = new Vraag(
                    0,
                    onderwerp,
                    TxtVraagInhoud.Text.Trim(),
                    CboMoeilijkheid.SelectedItem?.ToString() ?? "Open level",
                    true,
                    opties
                );

                
                _vraagManager.VoegNieuweVraagToe(nieuweVraag);

                MessageBox.Show("Vraag succesvol opgeslagen!",
                    "Opgeslagen", MessageBoxButton.OK, MessageBoxImage.Information);

               
                ResetFormulier();

                
                VulDropdowns();
            }
            catch (VraagExceptions ex)
            {
                
                MessageBox.Show(ex.Message, "Ongeldige invoer",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Onverwachte fout: {ex.Message}",
                    "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetFormulier()
        {
            TxtVraagInhoud.Clear();
            TxtOptieA.Clear();
            TxtOptieB.Clear();
            TxtOptieC.Clear();
            TxtOptieD.Clear();
            TxtOnderwerpNieuw.Clear();
            CboOnderwerpBestaand.SelectedIndex = -1;
            CboMoeilijkheid.SelectedIndex = 0;
            RbCorrectA.IsChecked = false;
            RbCorrectB.IsChecked = false;
            RbCorrectC.IsChecked = false;
            RbCorrectD.IsChecked = false;
        }

        

        private void LaadActieveVragen(string onderwerpFilter = null)
        {
            try
            {
                _actieveVragen.Clear();

                var onderwerpen = onderwerpFilter == null || onderwerpFilter == "Alle onderwerpen"
                    ? _vraagManager.GetAlleOnderwerpen()
                    : new List<string> { onderwerpFilter };

                foreach (var onderwerp in onderwerpen)
                {
                    
                    var vragen = _vraagManager.GetVragenByOnderwerp(onderwerp, alleenActief: false);
                    foreach (var v in VraagMapper.MapLijstNaarUI(vragen))
                        _actieveVragen.Add(v);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden vragen: {ex.Message}",
                    "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CboFilterOnderwerp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboFilterOnderwerp.SelectedItem is string gekozen)
                LaadActieveVragen(gekozen);
        }

        private void BtnDeactiveer_Click(object sender, RoutedEventArgs e)
        {
            if (DGActieveVragen.SelectedItem is not VraagUI geselecteerde)
            {
                MessageBox.Show("Selecteer eerst een vraag om te deactiveren.",
                    "Geen selectie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!geselecteerde.IsActief)
            {
                MessageBox.Show("Deze vraag is al gedeactiveerd.",
                    "Al inactief", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var bevestiging = MessageBox.Show(
                $"Wil je de volgende vraag deactiveren?\n\n\"{geselecteerde.Inhoud}\"\n\nDeze vraag zal niet meer beschikbaar zijn voor nieuwe testen.",
                "Bevestig deactivering", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (bevestiging != MessageBoxResult.Yes) return;

            try
            {
                
                _vraagManager.ArchiveerVraag(geselecteerde.Id);
                geselecteerde.IsActief = false;

                

                MessageBox.Show("Vraag succesvol gedeactiveerd.",
                    "Gedeactiveerd", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (VraagExceptions ex)
            {
                MessageBox.Show(ex.Message, "Fout",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}