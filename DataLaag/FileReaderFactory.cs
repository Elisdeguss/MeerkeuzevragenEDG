using BusinessLaag.Interfaces;
using DataLaag.FileReaders;

namespace DataLaag
{
    public class FileReaderFactory : IFileReaderFactory
    {
        public IFileReader GetReader(string filePath)
        {

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Het bestand op pad '{filePath}' werd niet gevonden.");
            }

            bool heeftAntwoordSleutelOnderaan = false;


            using (StreamReader sr = new StreamReader(filePath))
            {
                string regel;
                while ((regel = sr.ReadLine()) != null)
                {

                    if (regel.Trim().Equals("Antwoorden", StringComparison.OrdinalIgnoreCase))
                    {
                        heeftAntwoordSleutelOnderaan = true;
                        break;
                    }
                }
            }


            if (heeftAntwoordSleutelOnderaan)
            {

                return new Type2FileReader();
            }
            else
            {

                return new Type1FileReader();
            }
        }
    }
}