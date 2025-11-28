namespace TuCreditoMiVivienda.Api.CustomerContext.Domain;

public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, User User);