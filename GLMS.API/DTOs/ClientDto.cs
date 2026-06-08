namespace GLMS.API.DTOs;

public class ClientDto { public int ClientId { get; set; } public string Name { get; set; } = string.Empty; public string ContactDetails { get; set; } = string.Empty; public string Region { get; set; } = string.Empty; }
public class CreateClientDto { public string Name { get; set; } = string.Empty; public string ContactDetails { get; set; } = string.Empty; public string Region { get; set; } = string.Empty; }