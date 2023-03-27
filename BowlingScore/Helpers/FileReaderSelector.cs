using BowlingScore.FileReaders.Interfaces;

namespace BowlingScore.Helpers
{
    public static class FileReaderSelector
    {
        public static IFileReader SelectFileReader(string extension, IFileWrapper fileWrapper)
        {
            switch (extension)
            {
                case FileExtensions.TXT:
                    return new TxtFileReader(fileWrapper);

                default:
                    return null;
            }
        }
    }
}
