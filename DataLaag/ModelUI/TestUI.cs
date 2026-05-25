using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DataLaag.ModelUI
{
    public class TestUI
    {
        public TestUI(int id, string code, DateTime aanmaakDatum, List<VraagUI> vragen, List<string> onderwerpen)
        {
            Id = id;
            Code = code;
            AanmaakDatum = aanmaakDatum;
            Vragen = vragen;
            Onderwerpen = onderwerpen;
        }

        public int Id { get; set; }

        private string _code;
        public string Code
        {
            get { return _code; }
            set { _code = value; OnPropertyChanged(nameof(Code)); }
        }

        private DateTime _aanmaakDatum;
        public DateTime AanmaakDatum
        {
            get { return _aanmaakDatum; }
            set { _aanmaakDatum = value; OnPropertyChanged(nameof(AanmaakDatum)); }
        }

        private List<VraagUI> _vragen;
        public List<VraagUI> Vragen
        {
            get { return _vragen; }
            set { _vragen = value; OnPropertyChanged(nameof(Vragen)); }
        }

        private List<string> _onderwerpen;
        public List<string> Onderwerpen
        {
            get { return _onderwerpen; }
            set { _onderwerpen = value; OnPropertyChanged(nameof(Onderwerpen)); }
        }

       
        public string OnderwerpenWeergave => string.Join(", ", Onderwerpen);

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return Code;
        }
    }
}
