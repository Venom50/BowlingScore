using BowlingScore.FileReaders.Interfaces;
using BowlingScore.Helpers;
using NSubstitute;
using NUnit.Framework;

namespace BowlingScore.Tests.Helpers
{
    [TestFixture]
    class FileReaderSelectorTests
    {
        private IFileWrapper _fileWrapper;

        [SetUp]
        public void SetUp()
        {
            _fileWrapper = Substitute.For<IFileWrapper>();
        }

        [Test]
        public void SelectFileReader_WithTxtExtension_ReturnsTxtFileReaderInstance()
        {
            // Arrange
            var extension = ".txt";

            // Act
            var result = FileReaderSelector.SelectFileReader(extension, _fileWrapper);

            // Assert
            Assert.IsInstanceOf<TxtFileReader>(result);
        }

        [Test]
        public void SelectFileReader_WithUnknownExtension_ReturnsNull()
        {
            // Arrange
            var extension = ".jpeg";

            // Act
            var result = FileReaderSelector.SelectFileReader(extension, _fileWrapper);

            // Assert
            Assert.IsNull(result);
        }
    }
}
