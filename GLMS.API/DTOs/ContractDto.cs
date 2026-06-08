namespace GLMS.API.DTOs;

public class ContractDto { public int ContractId { get; set; } public int ClientId { get; set; } public string ClientName { get; set; } = string.Empty; public DateTime StartDate { get; set; } public DateTime EndDate { get; set; } public string Status { get; set; } = string.Empty; public string ServiceLevel { get; set; } = string.Empty; public string? SignedAgreementPath { get; set; } }
public class CreateContractDto { public int ClientId { get; set; } public DateTime StartDate { get; set; } public DateTime EndDate { get; set; } public string Status { get; set; } = "Draft"; public string ServiceLevel { get; set; } = string.Empty; }
public class UpdateContractStatusDto { public string Status { get; set; } = string.Empty; }