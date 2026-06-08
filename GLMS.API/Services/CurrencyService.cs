using System.Text.Json;
namespace GLMS.API.Services;

public class CurrencyService : ICurrencyService
{
    private readonly HttpClient _http;
    private readonly ILogger<CurrencyService> _logger;

    public CurrencyService(HttpClient http, ILogger<CurrencyService> logger)
    { _http = http; _logger = logger; }

    public async Task<decimal> GetUsdToZarRateAsync()
    {
        try
        {
            var json = await _http.GetStringAsync("https://open.er-api.com/v6/latest/USD");
            var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("rates").GetProperty("ZAR").GetDecimal();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch exchange rate, using fallback");
            return 18.5m;
        }
    }

    public decimal ConvertUsdToZar(decimal amount, decimal rate) =>
        Math.Round(amount * rate, 2);
}
