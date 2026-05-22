using System;
using System.Collections.Generic;
using System.Text;

namespace MeerkeuzeUI.ModelUI
{
    public class FeedbackResultaat
    {
        


            public string VraagInhoud { get; set; }
            public string GekozenAntwoord { get; set; }
            public string CorrectAntwoord { get; set; }
            public bool IsCorrect { get; set; }

            
            public string FeedbackDetails =>
                $"Jouw antwoord: {GekozenAntwoord}{Environment.NewLine}" +
                (IsCorrect
                    ? "✓ Correct!"
                    : $"✗ Niet correct, het moest \"{CorrectAntwoord}\" zijn.");
        }
    }



