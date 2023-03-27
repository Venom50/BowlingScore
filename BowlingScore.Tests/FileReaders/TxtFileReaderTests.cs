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
        public void ReadFile_FileIsEmpty_ReturnsError()
        {
            // Arrange
            _fileWrapper.Exists(FILE_PATH).Returns(true);
            _fileWrapper.ReadAllLines(FILE_PATH).Returns(new string[] { });

            // Act
            var result = _txtFileReader.ReadFile(FILE_PATH);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.IsSuccess);
                Assert.AreEqual("File is empty.", result.Messages[0]);
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

        [Test]
        public void ReadFile_CorrectFileStructure_ReturnsSuccess()
        {
            // Arrange
            var data = new[] { "name1", "1" };
            _fileWrapper.Exists(FILE_PATH).Returns(true);
            _fileWrapper.ReadAllLines(FILE_PATH).Returns(new[] { "name1", "1" });

            // Act
            var result = _txtFileReader.ReadFile(FILE_PATH);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual(0, result.Messages.Count);
                Assert.AreEqual(data, result.ResultObject);
            });
        }

        [TestCase("1")]
        [TestCase("/")]
        [TestCase("!")]
        public void GetBowlingData_InvalidName_ReturnsError(string name)
        {
            // Arrange
            var data = new[] { name, "1,2,3,4,5,6,7,8,9,10,10,9,8,7,6,5,4,3,2,1,0" };

            // Act
            var result = _txtFileReader.GetBowlingData(data);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.IsSuccess);
                Assert.AreEqual("Incorrect name in line 1 in file.", result.Messages[0]);
                Assert.IsNull(result.ResultObject);
            });
        }

        [TestCase(null)]
        [TestCase("")]
        public void GetBowlingData_NameMissing_ReturnsError(string name)
        {
            // Arrange
            var data = new[] { name, "1,2,3,4,5,6,7,8,9,10,10,9,8,7,6,5,4,3,2,1,0" };

            // Act
            var result = _txtFileReader.GetBowlingData(data);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.IsSuccess);
                Assert.AreEqual("Name missing in line 1 in file.", result.Messages[0]);
                Assert.IsNull(result.ResultObject);
            });
        }

        [TestCase("1,2,3,4,5,6,7,8,9,10,10,9,8,7,6,5,4,3,2,1,0,1")]
        [TestCase("1,2,3,4,5,6,7,8,9,10,10,9,8,7,6,5,4,3,2")]
        public void GetBowlingData_InvalidScoresLength_ReturnsError(string score)
        {
            // Arrange
            var data = new[] { "name", score };

            // Act
            var result = _txtFileReader.GetBowlingData(data);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.IsSuccess);
                Assert.AreEqual("Incorrect amount of throws in line 2 in file.", result.Messages[0]);
                Assert.IsNull(result.ResultObject);
            });
        }

        [TestCase("a,2,3,4,5,6,7,8,9,10,10,9,8,7,6,5,4,3,2,1")]
        [TestCase(",2,3,4,5,6,7,8,9,10,10,9,8,7,6,5,4,3,2,1")]
        [TestCase("/,2,3,4,5,6,7,8,9,10,10,9,8,7,6,5,4,3,2,1")]
        public void GetBowlingData_ScoreNaN_ReturnsError(string score)
        {
            // Arrange
            var data = new[] { "name", score };

            // Act
            var result = _txtFileReader.GetBowlingData(data);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.IsSuccess);
                Assert.AreEqual("Incorrect score format in line 2 in file.", result.Messages[0]);
                Assert.IsNull(result.ResultObject);
            });
        }

        [TestCase("11,2,3,4,5,6,7,8,9,10,10,9,8,7,6,5,4,3,2,1")]
        [TestCase("-1,2,3,4,5,6,7,8,9,10,10,9,8,7,6,5,4,3,2,1")]
        public void GetBowlingData_ScoreOutOfRange_ReturnsError(string score)
        {
            // Arrange
            var data = new[] { "name", score };

            // Act
            var result = _txtFileReader.GetBowlingData(data);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.IsSuccess);
                Assert.AreEqual("Score out of range in line 2 in file.", result.Messages[0]);
                Assert.IsNull(result.ResultObject);
            });
        }


        [Test]
        public void GetBowlingData_ValidScoreAfterStrike_ReturnsSuccess()
        {
            // Arrange
            var scoreList = new List<int>() { 10, 0, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 };
            var nameScoreKvpList = new List<KeyValuePair<string, List<int>>>();
            nameScoreKvpList.Add(new KeyValuePair<string, List<int>>("name", scoreList));
            var data = new[] { "name", "10,0,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2" };

            // Act
            var result = _txtFileReader.GetBowlingData(data);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual(0, result.Messages.Count);
                Assert.AreEqual(nameScoreKvpList, result.ResultObject);
            });
        }

        [Test]
        public void GetBowlingData_ValidScoreAfterStrikeInLastFrame_ReturnsSuccess()
        {
            // Arrange
            var scoreList = new List<int>() { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 10, 10, 10 };
            var nameScoreKvpList = new List<KeyValuePair<string, List<int>>>();
            nameScoreKvpList.Add(new KeyValuePair<string, List<int>>("name", scoreList));
            var data = new[] { "name", "1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,10,10,10" };

            // Act
            var result = _txtFileReader.GetBowlingData(data);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual(0, result.Messages.Count);
                Assert.AreEqual(nameScoreKvpList, result.ResultObject);
            });
        }

        [Test]
        public void GetBowlingData_InvalidScoreInFrame_ReturnsError()
        {
            // Arrange
            var data = new[] { "name", "6,5,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,10,10,10" };

            // Act
            var result = _txtFileReader.GetBowlingData(data);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.IsSuccess);
                Assert.AreEqual("Incorrect value of frame in line 2, value number 1 and 2 in file.", result.Messages[0]);
                Assert.IsNull(result.ResultObject);
            });
        }

        [TestCase("6,4,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,10,10")]
        [TestCase("6,4,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,10,0")]
        [TestCase("6,4,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,10,1")]
        public void GetBowlingData_InvalidScoreLastThrowForStrike_ReturnsError(string score)
        {
            // Arrange
            var data = new[] { "name", score };

            // Act
            var result = _txtFileReader.GetBowlingData(data);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.IsSuccess);
                Assert.AreEqual("There was a strike in first throw of last frame of the game in line 2 in file. Value from additional throw is required.", result.Messages[0]);
                Assert.IsNull(result.ResultObject);
            });
        }

        [Test]
        public void GetBowlingData_InvalidScoreLastThrowForSpare_ReturnsError()
        {
            // Arrange
            var data = new[] { "name", "6,4,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,6,4" };

            // Act
            var result = _txtFileReader.GetBowlingData(data);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.IsSuccess);
                Assert.AreEqual("There was a spare in last frame of the game in line 2 in file. Value from additional throw is required.", result.Messages[0]);
                Assert.IsNull(result.ResultObject);
            });
        }

        [TestCase("6,4,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,6,3,10")]
        [TestCase("6,4,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,0,10,10")]
        public void GetBowlingData_InvalidScoreLastFrameTooManyThrows_ReturnsError(string score)
        {
            // Arrange
            var data = new[] { "name", score };

            // Act
            var result = _txtFileReader.GetBowlingData(data);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.IsSuccess);
                Assert.AreEqual("There was no strike or spare in last frame of the game in line 2 in file. Value from additional throw should not be provided.", result.Messages[0]);
                Assert.IsNull(result.ResultObject);
            });
        }

        [Test]
        public void GetBowlingData_ValidData_ReturnsSuccess()
        {
            // Arrange
            var data = new[] { "name", "6,4,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,10,10,10" };
            var scoreList = new List<int>() { 6, 4, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 10, 10, 10 };
            var nameScoreKvpList = new List<KeyValuePair<string, List<int>>>();
            nameScoreKvpList.Add(new KeyValuePair<string, List<int>>("name", scoreList));

            // Act
            var result = _txtFileReader.GetBowlingData(data);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual(0, result.Messages.Count);
                Assert.AreEqual(nameScoreKvpList, result.ResultObject);
            });
        }
    }
}
