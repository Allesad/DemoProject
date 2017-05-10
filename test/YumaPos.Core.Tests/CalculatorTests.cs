using Xunit;

namespace YumaPos.Core.Tests
{
    public class CalculatorTests
    {
        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(7)]
        public void AddTest(int toAdd)
        {
            // Arrange
            var calc = new Calculator();

            // Act
            var res = calc.Add(toAdd);

            // Assert
            Assert.Equal(toAdd, res);
        }

        [Theory]
        [InlineData(2,3,5)]
        [InlineDataAttribute(4,8,12)]
        [InlineDataAttribute(-2,14,12)]
        public void ResultTest(int first, int second, int result)
        {
            // Arrange
            var calc = new Calculator();

            // Act
            calc.Add(first);
            calc.Add(second);

            // Assert
            Assert.Equal(result, calc.CalculateResult());
        }
    }
}