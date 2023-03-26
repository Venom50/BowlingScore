using BowlingScore.Calculator;
using NUnit.Framework;
using System.Collections.Generic;

namespace BowlingScore.Tests.Calculator
{
    [TestFixture]
    class BowlingScoreCalculatorTests
    {
        private BowlingScoreCalculator _bowlingScoreCalculator;

        [SetUp]
        public void SetUp()
        {
            _bowlingScoreCalculator = new BowlingScoreCalculator();
        }

        [Test]
        public void CalculateScore_AllOpenFrames_ReturnCorrectScore()
        {
            // Arrange
            var throws = new List<int>() { 3, 5, 2, 5, 7, 1, 4, 4, 6, 2, 8, 1, 9, 0, 1, 4, 6, 2, 3, 5 };
            int expectedScore = 78;

            // Act
            var score = _bowlingScoreCalculator.CalculateScore(throws);

            // Assert
            Assert.AreEqual(expectedScore, score);
        }

        [Test]
        public void CalculateScore_AllSpares_ReturnCorrectScore()
        {
            // Arrange
            var throws = new List<int>() { 5, 5, 2, 8, 7, 3, 1, 9, 6, 4, 8, 2, 9, 1, 3, 7, 5, 5, 2, 8, 7 };
            int expectedScore = 150;

            // Act
            var score = _bowlingScoreCalculator.CalculateScore(throws);

            // Assert
            Assert.AreEqual(expectedScore, score);
        }

        [Test]
        public void CalculateScore_AllStrikes_ReturnCorrectScore()
        {
            // Arrange
            var throws = new List<int>() { 10, 0, 10, 0, 10, 0, 10, 0, 10, 0, 10, 0, 10, 0, 10, 0, 10, 0, 10, 10, 10 };
            int expectedScore = 300;

            // Act
            var score = _bowlingScoreCalculator.CalculateScore(throws);

            // Assert
            Assert.AreEqual(expectedScore, score);
        }

        [Test]
        public void CalculateScore_MixedFrames_ReturnCorrectScore()
        {
            // Arrange
            var throws = new List<int>() { 6, 2, 10, 0, 4, 6, 3, 4, 8, 1, 5, 5, 1, 8, 2, 6, 10, 0, 3, 7, 4 };
            int expectedScore = 119;

            // Act
            var score = _bowlingScoreCalculator.CalculateScore(throws);

            // Assert
            Assert.AreEqual(expectedScore, score);
        }
    }
}
