namespace TuCreditoMiVivienda.Api.CustomerContext.Domain;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = "asesor";
}