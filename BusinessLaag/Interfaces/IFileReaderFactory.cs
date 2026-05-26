namespace BusinessLaag.Interfaces
{
    public interface IFileReaderFactory
    {
        IFileReader GetReader(string filePath);
    }
}
