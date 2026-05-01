namespace SCADA.Modelos;

public class OperacionResumenDTO
{
    public int Id { get; set; }
    public int? IdMaquina { get; set; }
    public string CodigoOperacion { get; set; } = "";
    public string NombreOrden { get; set; } = "";
    public string NombreMaquina { get; set; } = "";
    public string Producto { get; set; } = "";
    public int CiclosObjetivo { get; set; }
    public int PiezasFabricadas { get; set; } = 0;
    public int PiezasRotas { get; set; } = 0;
    public string Estado { get; set; } = "";
    public int EstadoMaquinaId { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}