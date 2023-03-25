using BowlingScore.FileReaders.Interfaces;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace BowlingScore.Tests.FileReaders
{
    [TestFixture]
    class TxtFileReaderTests
    {
        private const string FILE_PATH = "filePath.txt";

        private TxtFileReader _txtFileReader;
        private IFileWrapper _fileWrapper;

        [SetUp]
        public void SetUp()
        {
            _fileWrapper = Substitute.For<IFileWrapper>();
            _txtFileReader = new TxtFileReader(_fileWrapper);
        }

        [Test]
        public void ReadFile_InvalidFile_ReturnsError()
        {
            // Arrange
            _fileWrapper.Exists(FILE_PATH).Returns(false);

            // Act
            var result = _txtFileReader.ReadFile(FILE_PATH);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.IsSuccess);
                Assert.AreEqual("File does not exist.", result.Messages[0]);
                Assert.IsNull(result.ResultObject);
            });
        }

        [Test]
        public void ReadFile_IncorrectFileStructure_ReturnsError()
        {
            // Arrange
            _fileWrapper.Exists(FILE_PATH).Returns(true);
            _fileWrapper.ReadAllLines(FILE_PATH).Returns(new[] { "name1", "1", "name2" });

            // Act
            var result = _txtFileReader.ReadFile(FILE_PATH);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.IsSuccess);
                Assert.AreEqual("Incorrect file structure.", result.Messages[0]);
                Assert.IsNull(result.ResultObject);
            });
        }

        [TestCase("1")]
        [TestCase("/")]
        [TestCase("!")]
        public void ReadFile_InvalidName_ReturnsErrorResult(string name)
        {
            // Arrange
            _fileWrapper.Exists(FILE_PATH).Returns(true);
            _fileWrapper.ReadAllLines(FILE_PATH).Returns(new[] { name, "1,2,3,4,5,6,7,8,9,10,10,9,8,7,6,5,4,3,2,1,0"});

            // Act
            var result = _txtFileReader.ReadFile(FILE_PATH);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Incorrect name in line 0 in file.", result.Messages[0]);
            Assert.IsNull(result.ResultObject);
        }

        [TestCase(null)]
        [TestCase("")]
        public void ReadFile_NameMissing_ReturnsErrorResult(string name)
        {
            // Arrange
            _fileWrapper.Exists(FILE_PATH).Returns(true);
            _fileWrapper.ReadAllLines(FILE_PATH).Returns(new[] { name, "1,2,3,4,5,6,7,8,9,10,10,9,8,7,6,5,4,3,2,1,0" });

            // Act
            var result = _txtFileReader.ReadFile(FILE_PATH);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Name missing in line 0 in file.", result.Messages[0]);
            Assert.IsNull(result.ResultObject);
        }

        [TestCase("1,2,3,4,5,6,7,8,9,10,10,9,8,7,6,5,4,3,2,1,0,1")]
        [TestCase("1,2,3,4,5,6,7,8,9,10,10,9,8,7,6,5,4,3,2")]
        public void ReadFile_InvalidScoresLength_ReturnsErrorResult(string score)
        {
            // Arrange
            _fileWrapper.Exists(FILE_PATH).Returns(true);
            _fileWrapper.ReadAllLines(FILE_PATH).Returns(new[] { "name", score });

            // Act
            var result = _txtFileReader.ReadFile(FILE_PATH);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Incorrect amount of throws in line 1 in file.", result.Messages[0]);
            Assert.IsNull(result.ResultObject);
        }

        [TestCase("a,2,3,4,5,6,7,8,9,10,10,9,8,7,6,5,4,3,2,1")]
        [TestCase(",2,3,4,5,6,7,8,9,10,10,9,8,7,6,5,4,3,2,1")]
        [TestCase("/,2,3,4,5,6,7,8,9,10,10,9,8,7,6,5,4,3,2,1")]
        public void ReadFile_ScoreNaN_ReturnsErrorResult(string score)
        {
            // Arrange
            _fileWrapper.Exists(FILE_PATH).Returns(true);
            _fileWrapper.ReadAllLines(FILE_PATH).Returns(new[] { "name", score });

            // Act
            var result = _txtFileReader.ReadFile(FILE_PATH);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Incorrect score format in line 1 in file.", result.Messages[0]);
            Assert.IsNull(result.ResultObject);
        }

        [TestCase("11,2,3,4,5,6,7,8,9,10,10,9,8,7,6,5,4,3,2,1")]
        [TestCase("-1,2,3,4,5,6,7,8,9,10,10,9,8,7,6,5,4,3,2,1")]
        public void ReadFile_ScoreOutOfRange_ReturnsErrorResult(string score)
        {
            // Arrange
            _fileWrapper.Exists(FILE_PATH).Returns(true);
            _fileWrapper.ReadAllLines(FILE_PATH).Returns(new[] { "name", score });

            // Act
            var result = _txtFileReader.ReadFile(FILE_PATH);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Score out of range in line 1 in file.", result.Messages[0]);
            Assert.IsNull(result.ResultObject);
        }

        [Test]
        public void ReadFile_CorrectData_ReturnsSuccessResult()
        {
            // Arrange
            var scoreList = new List<string>() { "1", "2", "3" ,"4", "5", "6", "7", "8", "9", "10", "10", "9", "8", "7", "6", "5", "4", "3", "2", "1" };
            var nameScoreKvpList = new List<KeyValuePair<string, List<string>>>();
            nameScoreKvpList.Add(new KeyValuePair<string, List<string>>("name", scoreList));

            _fileWrapper.Exists(FILE_PATH).Returns(true);
            _fileWrapper.ReadAllLines(FILE_PATH).Returns(new[] { "name", "1,2,3,4,5,6,7,8,9,10,10,9,8,7,6,5,4,3,2,1" });

            // Act
            var result = _txtFileReader.ReadFile(FILE_PATH);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0, result.Messages.Count);
            Assert.AreEqual(nameScoreKvpList, result.ResultObject);
        }
    }
}
