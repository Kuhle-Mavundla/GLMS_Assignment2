namespace GLMS.API.Services;
public interface ICurrencyService
{
    Task<decimal> GetUsdToZarRateAsync();
    decimal ConvertUsdToZar(decimal amount, decimal rate);
}
