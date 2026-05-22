using BusinessLaag.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLaag.Interfaces
{
    public interface IFileReader
    {

        List<Vraag> ExtractQuestionsFromFile(string filePath);
    }
}
