using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCADA.Modelos;

[Table("ImputacionOperarios")]
public class ImputacionOperario
{
    [Key]
    public int Id { get; set; }
    public int IdOperacion { get; set; }
    public int IdEmpleado { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Horas { get; set; }
    public string? Observaciones { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int PiezasFabricadas { get; set; } = 0;
    public int PiezasRotas { get; set; } = 0;

    [ForeignKey("IdOperacion")]
    public virtual OperacionesOrden Operacion { get; set; } = null!;

    [ForeignKey("IdEmpleado")]
    public virtual Empleado Empleado { get; set; } = null!;
}
