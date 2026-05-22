using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLaag.Interfaces
{
    public interface IFileReaderFactory
    {
        IFileReader GetReader(string filePath);
    }
}
