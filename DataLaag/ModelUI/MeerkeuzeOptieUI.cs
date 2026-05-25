using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DataLaag.ModelUI
{
    public class MeerkeuzeOptieUI
    {
        public MeerkeuzeOptieUI(int id, string tekst, bool isCorrect)
        {
            Id = id;
            Tekst = tekst;
            IsCorrect = isCorrect;
        }

        public int Id { get; set; }

        private string _tekst;
        public string Tekst
        {
            get { return _tekst; }
            set { _tekst = value; OnPropertyChanged(nameof(Tekst)); }
        }

        private bool _isCorrect;
        public bool IsCorrect
        {
            get { return _isCorrect; }
            set { _isCorrect = value; OnPropertyChanged(nameof(IsCorrect)); }
        }

        
        private bool _isGeselecteerd;
        public bool IsGeselecteerd
        {
            get { return _isGeselecteerd; }
            set { _isGeselecteerd = value; OnPropertyChanged(nameof(IsGeselecteerd)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return Tekst;
        }
    }
}