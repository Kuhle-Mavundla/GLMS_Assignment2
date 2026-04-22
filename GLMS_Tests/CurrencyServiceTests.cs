using Xunit;
using GLMS_Assignment2.Services;
using Microsoft.Extensions.Logging;

namespace GLMS_Tests
{
    /// <summary>
    /// Tests for currency conversion logic.
    /// Verifies that the math converting USD to ZAR is correct given a specific rate.
    /// </summary>
    public class CurrencyServiceTests
    {
        private readonly CurrencyService _service;

        public CurrencyServiceTests()
        {
            // We test the pure conversion method — no HTTP needed
            _service = CreateCurrencyServiceForTesting();
        }

        private static CurrencyService CreateCurrencyServiceForTesting()
        {
            // Use reflection or direct instantiation for the conversion method
            // Since ConvertUsdToZar is a pure method, we can test it directly
            var httpClient = new HttpClient();
            var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder().Build();
            var logger = Microsoft.Extensions.Logging.LoggerFactory.Create(b => { }).CreateLogger<CurrencyService>();
            return new CurrencyService(httpClient, config, logger);
        }

        [Fact]
        public void ConvertUsdToZar_ValidAmount_ReturnsCorrectZar()
        {
            // Arrange
            decimal usd = 100m;
            decimal rate = 18.50m;

            // Act
            decimal result = _service.ConvertUsdToZar(usd, rate);

            // Assert
            Assert.Equal(1850.00m, result);
        }

        [Fact]
        public void ConvertUsdToZar_SmallAmount_RoundsCorrectly()
        {
            // Arrange
            decimal usd = 1.99m;
            decimal rate = 18.3456m;

            // Act
            decimal result = _service.ConvertUsdToZar(usd, rate);

            // Assert — 1.99 * 18.3456 = 36.507744, rounded to 36.51
            Assert.Equal(36.51m, result);
        }

        [Fact]
        public void ConvertUsdToZar_LargeAmount_ReturnsCorrectValue()
        {
            // Arrange
            decimal usd = 50000m;
            decimal rate = 18.25m;

            // Act
            decimal result = _service.ConvertUsdToZar(usd, rate);

            // Assert
            Assert.Equal(912500.00m, result);
        }

        [Fact]
        public void ConvertUsdToZar_ZeroRate_ThrowsArgumentException()
        {
            // Arrange
            decimal usd = 100m;
            decimal rate = 0m;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.ConvertUsdToZar(usd, rate));
        }

        [Fact]
        public void ConvertUsdToZar_NegativeRate_ThrowsArgumentException()
        {
            // Arrange
            decimal usd = 100m;
            decimal rate = -5m;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.ConvertUsdToZar(usd, rate));
        }

        [Fact]
        public void ConvertUsdToZar_ZeroAmount_ReturnsZero()
        {
            // Edge case: 0 USD
            decimal result = _service.ConvertUsdToZar(0m, 18.50m);
            Assert.Equal(0m, result);
        }

        [Fact]
        public void ConvertUsdToZar_VerySmallAmount_HandlesCorrectly()
        {
            // Edge case: tiny amount
            decimal usd = 0.01m;
            decimal rate = 18.50m;

            decimal result = _service.ConvertUsdToZar(usd, rate);

            // Compute expected using the same midpoint rounding used by the implementation (ToEven)
            decimal expected = Math.Round(usd * rate, 2, MidpointRounding.ToEven);
            Assert.Equal(expected, result);
        }
    }
}