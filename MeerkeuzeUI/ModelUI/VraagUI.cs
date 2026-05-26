using System.ComponentModel;

namespace MeerkeuzeUI.ModelUI
{
    public class VraagUI:INotifyPropertyChanged
    {

        public VraagUI(int id, string onderwerp, string inhoud, string moeilijkheidsgraad, bool isActief, List<MeerkeuzeOptieUI> keuzes)
        {
            Id = id;
            Onderwerp = onderwerp;
            Inhoud = inhoud;
            Moeilijkheidsgraad = moeilijkheidsgraad;
            IsActief = isActief;
            Keuzes = keuzes;
        }

        public int Id { get; set; }

        private string _onderwerp;
        public string Onderwerp
        {
            get { return _onderwerp; }
            set { _onderwerp = value; OnPropertyChanged(nameof(Onderwerp)); }
        }

        private string _inhoud;
        public string Inhoud
        {
            get { return _inhoud; }
            set { _inhoud = value; OnPropertyChanged(nameof(Inhoud)); }
        }

        private string _moeilijkheidsgraad;
        public string Moeilijkheidsgraad
        {
            get { return _moeilijkheidsgraad; }
            set { _moeilijkheidsgraad = value; OnPropertyChanged(nameof(Moeilijkheidsgraad)); }
        }

        private bool _isActief;
        public bool IsActief
        {
            get { return _isActief; }
            set { _isActief = value; OnPropertyChanged(nameof(IsActief)); }
        }

        private List<MeerkeuzeOptieUI> _keuzes;
        public List<MeerkeuzeOptieUI> Keuzes
        {
            get { return _keuzes; }
            set { _keuzes = value; OnPropertyChanged(nameof(Keuzes)); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return Inhoud;
        }
    }
}

