namespace GLMS.API.DTOs;

public class ServiceRequestDto { public int ServiceRequestId { get; set; } public int ContractId { get; set; } public string Description { get; set; } = string.Empty; public decimal CostUSD { get; set; } public decimal CostZAR { get; set; } public decimal ExchangeRateUsed { get; set; } public string Status { get; set; } = string.Empty; public DateTime CreatedAt { get; set; } }
public class CreateServiceRequestDto { public int ContractId { get; set; } public string Description { get; set; } = string.Empty; public decimal CostUSD { get; set; } }
public class LoginDto { public string Username { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; }
public class TokenResponseDto { public string Token { get; set; } = string.Empty; public DateTime Expiry { get; set; } }