using Core.Services;
using Core.ServicesContracts;

namespace Tests.ServicesTests
{
    public class ShortenerServiceTests
    {
        private readonly IShortenerService _service;

        public ShortenerServiceTests()
        {
            _service = new ShortenerService();
        }

        [Fact]
        public void Encode_ZeroInput_ShouldReturnZero()
        {
            // Arrange
            int input = 0;

            // Act
            string actual = _service.Encode(input);

            // Assert
            Assert.Equal("0", actual);
        }

        [Fact]
        public void Encode_AnyNonZeroInput_ShouldReturnCorrectEncodedValue()
        {
            // Arrange
            int input = 1000;

            // Act
            string actual = _service.Encode(input);

            // Assert
            Assert.Equal("G8", actual);
        }

        [Fact]
        public void Decode_AnyInput_ShouldReturnCorrectDecodedValue()
        {
            // Arrange
            string input = "G8";

            // Act
            int actual = _service.Decode(input);

            // Assert
            Assert.Equal(1000, actual);
        }
    }
}
