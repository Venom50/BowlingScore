using BowlingScore.Calculator;
using BowlingScore.Controller;
using BowlingScore.Generic;
using BowlingScore.Models;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace BowlingScore.Tests.Controllers
{
    [TestFixture]
    class BowlingDataControllerTests
    {
        private const string FILE_PATH = "filePath.txt";

        private BowlingScoreCalculator _bowlingScoreCalculator;
        private IFileReader _fileReader;
        private BowlingDataController _bowlingDataController;

        [SetUp]
        public void SetUp()
        {
            _fileReader = Substitute.For<IFileReader>();
            _bowlingScoreCalculator = Substitute.For<BowlingScoreCalculator>();
            _bowlingDataController = new BowlingDataController(_bowlingScoreCalculator);
        }

        [Test]
        public void GetBowlingScoreModelsFromFile_WithUnsupportedExtension_ReturnsErrorResult()
        {
            // Act
            var result = _bowlingDataController.GetBowlingScoreModelsFromFile(FILE_PATH, null);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.IsSuccess);
                Assert.AreEqual("No matching file readers for this file's extension.", result.Messages[0]);
            });
        }

        [Test]
        public void GetBowlingScoreModelsFromFile_ReadFileFailed_ReturnsErrorResult()
        {
            // Arrange
            var fileResult = new Result { IsSuccess = false, Messages = new List<string> { "File Error." } };
            _fileReader.ReadFile(FILE_PATH).Returns(fileResult);

            // Act
            var result = _bowlingDataController.GetBowlingScoreModelsFromFile(FILE_PATH, _fileReader);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.IsSuccess);
                Assert.AreEqual("File Error.", result.Messages[0]);
            });
        }

        [Test]
        public void GetBowlingScoreModelsFromFile_GetBowlingDataFailed_ReturnsErrorResult()
        {
            // Arrange
            var fileContent = new string[] { "test" };
            var fileResult = new Result { IsSuccess = true, ResultObject = fileContent };
            _fileReader.ReadFile(FILE_PATH).Returns(fileResult);

            var bowlingData = new Result { IsSuccess = false, Messages = new List<string> { "Data Error." } };
            _fileReader.GetBowlingData(fileContent).Returns(bowlingData);

            // Act
            var result = _bowlingDataController.GetBowlingScoreModelsFromFile(FILE_PATH, _fileReader);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.IsSuccess);
                Assert.AreEqual("Data Error.", result.Messages[0]);
            });
        }

        [Test]
        public void GetBowlingScoreModelsFromFile_AllResultsSucceeded_ReturnsSuccessResult()
        {
            // Arrange
            var fileContent = new string[] { "test" };
            var fileResult = new Result { IsSuccess = true, ResultObject = fileContent };
            _fileReader.ReadFile(FILE_PATH).Returns(fileResult);

            var bowlingData = new Result { IsSuccess = true, ResultObject = new List<KeyValuePair<string, List<int>>> { } };
            _fileReader.GetBowlingData(fileContent).Returns(bowlingData);

            // Act
            var result = _bowlingDataController.GetBowlingScoreModelsFromFile(FILE_PATH, _fileReader);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual(0, result.Messages.Count);
                Assert.AreEqual(bowlingData.ResultObject, result.ResultObject);
            });
        }

        [Test]
        public void AddBowlingScoreModelsToList_WithListOfKvp_ReturnsCorrectBowlingScoreModels()
        {
            // Assert
            var nameScoreKvp = new List<KeyValuePair<string, List<int>>>
            {
                new KeyValuePair<string, List<int>>("Krzysztof Król", new List<int> { 9, 1, 9, 1, 9, 1, 9, 1, 9, 1, 9, 1, 9, 1, 9, 1, 9, 1, 9, 1, 9 }),
                new KeyValuePair<string, List<int>>("Marcin Król", new List<int> { 10, 0, 10, 0, 10, 0, 10, 0, 10, 0, 10, 0, 10, 0, 10, 0, 10, 0, 10, 10, 10 })
            };

            var expectedScoreModels = new List<BowlingScoreModel>
            {
                new BowlingScoreModel
                {
                    Name = "Krzysztof Król",
                    ThrowsScores = nameScoreKvp[0].Value,
                    TotalScore = 190
                },
                new BowlingScoreModel
                {
                    Name = "Marcin Król",
                    ThrowsScores = nameScoreKvp[1].Value,
                    TotalScore = 300
                }
            };

            // Act
            var result = _bowlingDataController.AddBowlingScoreModelsToList(nameScoreKvp);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedScoreModels[0].Name, result[0].Name);
                Assert.AreEqual(expectedScoreModels[0].ThrowsScores, result[0].ThrowsScores);
                Assert.AreEqual(expectedScoreModels[0].TotalScore, result[0].TotalScore);
                Assert.AreEqual(expectedScoreModels[1].Name, result[1].Name);
                Assert.AreEqual(expectedScoreModels[1].ThrowsScores, result[1].ThrowsScores);
                Assert.AreEqual(expectedScoreModels[1].TotalScore, result[1].TotalScore);
            });
        }
    }
}
