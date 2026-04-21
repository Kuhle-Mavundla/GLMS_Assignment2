namespace GLMS_Assignment2.Services.Interfaces
{ public interface ICurrencyService { Task<decimal> GetUsdToZarRateAsync(); decimal ConvertUsdToZar(decimal amountUsd, decimal exchangeRate); } }