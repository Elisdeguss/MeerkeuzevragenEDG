using System.ComponentModel;

namespace MeerkeuzeUI.ModelUI
{
    public class MeerkeuzeOptieUI : INotifyPropertyChanged
    {
        public MeerkeuzeOptieUI(int id, string tekst, bool isCorrect, int vraagId)
        {
            Id = id;
            Tekst = tekst;
            IsCorrect = isCorrect;
            VraagId = vraagId;
        }

        public int Id { get; set; }

        
        public int VraagId { get; set; }

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

        public override string ToString() => Tekst;
    }
}