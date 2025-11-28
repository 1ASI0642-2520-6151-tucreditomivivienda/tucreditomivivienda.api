namespace TuCreditoMiVivienda.Api.CustomerContext.Domain;

public class Client
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string EstadoCivil { get; set; } = string.Empty;
    public decimal IngresosMensuales { get; set; }
    public int Dependientes { get; set; }
    public string ActividadLaboral { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UnidadInteres { get; set; } = string.Empty;
}