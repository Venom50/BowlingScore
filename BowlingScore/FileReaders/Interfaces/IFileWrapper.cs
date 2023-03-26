namespace BowlingScore.FileReaders.Interfaces
{
    public interface IFileWrapper
    {
        string[] ReadAllLines(string filePath);

        bool Exists(string filePath);
    }
}