namespace TuCreditoMiVivienda.Api.LoanContext.Domain;

public class Property
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Proyecto { get; set; } = string.Empty;
    public string Torre { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string TipoUnidad { get; set; } = string.Empty;
    public decimal AreaM2 { get; set; }
    public int Dormitorios { get; set; }
    public int Banos { get; set; }
    public decimal PrecioVenta { get; set; }
    public string Estado { get; set; } = "disponible";
}