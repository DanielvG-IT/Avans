using CoreLink.WebApi.Models;

namespace CoreLink.Tests
{
    [TestClass]
    public class Environment2DTests
    {
        [TestMethod]
        [DataRow(0, 0, true)]     // Center of the environment
        [DataRow(100, 50, true)]  // Maximum valid boundary
        [DataRow(-100, -50, true)]// Minimum valid boundary
        [DataRow(99.9f, 49.9f, true)] // Just inside the boundary
        [DataRow(101, 50, false)] // Just outside the maxLength boundary
        [DataRow(100, 51, false)] // Just outside the maxHeight boundary
        [DataRow(-101, -50, false)] // Just outside the minLength boundary
        [DataRow(0, -51, false)]  // Just outside the minHeight boundary
        [DataRow(150, 0, false)]  // Way outside the boundary (X)
        [DataRow(0, 70, false)]   // Way outside the boundary (Y)]

        public void IsPositionValid_ShouldReturnExpectedResult(float x, float y, bool expected)
        {
            // Arrange
            var env = new Environment2D
            {
                maxLength = 200,  // Max valid range: -100 to 100
                maxHeight = 100   // Max valid range: -50 to 50
            };

            // Act
            var result = env.IsPositionValid(x, y);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
