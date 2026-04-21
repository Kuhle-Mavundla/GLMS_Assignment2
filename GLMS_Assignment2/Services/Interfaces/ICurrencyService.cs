namespace GLMS.Web.Services.Interfaces
{ public interface ICurrencyService { Task<decimal> GetUsdToZarRateAsync(); decimal ConvertUsdToZar(decimal amountUsd, decimal exchangeRate); } }