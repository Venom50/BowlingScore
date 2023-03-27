using BowlingScore.Generic;
using NUnit.Framework;

namespace BowlingScore.Tests
{
    [TestFixture]
    class ResultTests
    {
        [Test]
        public void AddError_WithErrorMessage_ReturnsIsSuccessToFalseAndErrorMessage()
        {
            // Arrange
            var result = new Result();
            var errorMessage = "Error message";

            // Act
            result.AddError(errorMessage);

            // Arrange
            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.IsSuccess);
                Assert.Contains(errorMessage, result.Messages);
            });
        }

        [Test]
        public void AddError_WithMultipleErrorMessages_ReturnsIsSuccessToFalseAndErrorMessage()
        {
            // Arrange
            var result = new Result();
            var errorMessage1 = "Error message 1";
            var errorMessage2 = "Error message 2";

            // Act
            result.AddError(errorMessage1);
            result.AddError(errorMessage2);

            // Arrange
            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.IsSuccess);
                Assert.Contains(errorMessage1, result.Messages);
                Assert.Contains(errorMessage2, result.Messages);
            });
        }

        [Test]
        public void AddInfo_WithInfoMessage_ReturnsInfoMessage()
        {
            // Arrange
            var result = new Result();
            var infoMessage = "Info message.";

            // Act
            result.AddInfo(infoMessage);

            // Assert
            Assert.Contains(infoMessage, result.Messages);
        }

        [Test]
        public void AddInfo_WithMultipleInfoMessages_ReturnsInfoMessage()
        {
            // Arrange
            var result = new Result();
            var infoMessage1 = "Info message 1.";
            var infoMessage2 = "Info message 2.";

            // Act
            result.AddInfo(infoMessage1);
            result.AddInfo(infoMessage2);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.Contains(infoMessage1, result.Messages);
                Assert.Contains(infoMessage2, result.Messages);
            });
        }


        [Test]
        public void Messages_ByDefault_IsNotNull()
        {
            // Arrange
            var result = new Result();

            // Assert
            Assert.IsNotNull(result.Messages);
        }

        [Test]
        public void ResultObject_ByDefault_IsNull()
        {
            // Arrange
            var result = new Result();

            // Assert
            Assert.IsNull(result.ResultObject);
        }

        [Test]
        public void ResultObject_WithSetObject_ReturnsObject()
        {
            // Arrange
            var result = new Result();
            var expectedResultObject = new object();

            // Act
            result.ResultObject = expectedResultObject;
            var actualResultObject = result.ResultObject;

            // Assert
            Assert.AreEqual(expectedResultObject, actualResultObject);
        }
    }
}
