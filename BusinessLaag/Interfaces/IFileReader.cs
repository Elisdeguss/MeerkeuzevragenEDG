using BusinessLaag.Model;

namespace BusinessLaag.Interfaces
{
    public interface IFileReader
    {

        List<Vraag> ExtractQuestionsFromFile(string filePath);
    }
}
