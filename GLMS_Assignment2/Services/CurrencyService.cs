using GLMS_Assignment2.Services.Interfaces;
using Newtonsoft.Json.Linq;

namespace GLMS_Assignment2.Services
{
    // This service connects to the ExchangeRate-API to get live USD to ZAR rates.
    // It uses HttpClient (injected via Dependency Injection) to make the API call.
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;    // Used to make HTTP requests
        private readonly string _apiUrl;            // The API endpoint URL
        private readonly ILogger<CurrencyService> _logger;  // For logging messages

        // Constructor: receives dependencies through Dependency Injection
        public CurrencyService(HttpClient httpClient, IConfiguration config, ILogger<CurrencyService> logger)
        {
            _httpClient = httpClient;
            // Read the API URL from appsettings.json, or use a default
            _apiUrl = config["ExchangeRateApi:BaseUrl"] ?? "https://api.exchangerate-api.com/v4/latest/USD";
            _logger = logger;
        }

        // Calls the external API and returns the current USD to ZAR rate
        public async Task<decimal> GetUsdToZarRateAsync()
        {
            try
            {
                // Make the HTTP GET request to the exchange rate API
                var response = await _httpClient.GetStringAsync(_apiUrl);

                // Parse the JSON response to extract the ZAR rate
                var json = JObject.Parse(response);
                var zarRate = json["rates"]?["ZAR"]?.Value<decimal>();

                // Check that we got a valid rate
                if (zarRate == null || zarRate <= 0)
                    throw new InvalidOperationException("Could not get ZAR rate from the API.");

                _logger.LogInformation("Successfully fetched USD to ZAR rate: {Rate}", zarRate);
                return zarRate.Value;
            }
            catch (HttpRequestException ex)
            {
                // This catches network errors (e.g. API is down, no internet)
                _logger.LogError(ex, "Could not reach the ExchangeRate API.");
                throw new InvalidOperationException("Currency service is unavailable. Please try again later.", ex);
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                // Catch any other unexpected errors
                _logger.LogError(ex, "Unexpected error while fetching exchange rate.");
                throw new InvalidOperationException("An unexpected error occurred.", ex);
            }
        }

        // Converts a USD amount to ZAR using the given exchange rate.
        // This is a pure math function with no API calls.
        public decimal ConvertUsdToZar(decimal amountUsd, decimal exchangeRate)
        {
            // Make sure the rate is valid
            if (exchangeRate <= 0)
                throw new ArgumentException("Exchange rate must be greater than zero.", nameof(exchangeRate));

            // Multiply and round to 2 decimal places
            return Math.Round(amountUsd * exchangeRate, 2);
        }
    }
}