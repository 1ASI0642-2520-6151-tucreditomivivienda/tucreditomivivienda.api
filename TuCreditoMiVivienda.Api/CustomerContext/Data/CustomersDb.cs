using TuCreditoMiVivienda.Api.CustomerContext.Domain;

namespace TuCreditoMiVivienda.Api.CustomerContext.Data;

public class CustomersDb
{
    public List<Client> Clients { get; } = new();

    public CustomersDb()
    {
        // Datos demo
        Clients.Add(new Client
        {
            Nombres = "Juan",
            Apellidos = "Pérez",
            Documento = "12345678",
            EstadoCivil = "Soltero",
            IngresosMensuales = 3500,
            Dependientes = 0,
            ActividadLaboral = "Dependiente",
            Telefono = "999999999",
            Email = "juan@example.com",
            UnidadInteres = "Proyecto A - Dpto 301"
        });
    }
}