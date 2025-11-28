using TuCreditoMiVivienda.Api.LoanContext.Domain;

namespace TuCreditoMiVivienda.Api.LoanContext.Data;

public class RealEstateDb
{
    public List<Property> Properties { get; } = new();

    public RealEstateDb()
    {
        Properties.Add(new Property
        {
            Proyecto = "Residencial Callao",
            Torre = "A",
            Numero = "301",
            Ciudad = "Lima",
            TipoUnidad = "Departamento",
            AreaM2 = 75,
            Dormitorios = 3,
            Banos = 2,
            PrecioVenta = 250_000,
            Estado = "disponible"
        });
    }
}