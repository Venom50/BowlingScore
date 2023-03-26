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
