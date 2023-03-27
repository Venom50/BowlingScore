using BowlingScore.FileReaders.Interfaces;
using System.IO;

namespace BowlingScore.FileReaders
{
    public class FileWrapper : IFileWrapper
    {
        public string[] ReadAllLines(string filePath)
        {
            return File.ReadAllLines(filePath);
        }

        public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
