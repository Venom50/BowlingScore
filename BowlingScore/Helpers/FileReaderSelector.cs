namespace BowlingScore.Helpers
{
    public static class FileReaderSelector
    {
        internal static IFileReader SelectFileReader(string extension)
        {
            switch (extension)
            {
                case FileExtensions.TXT:
                    return new TxtFileReader();

                default:
                    return null;
            }
        }
    }
}
