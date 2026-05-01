using System.ComponentModel.DataAnnotations.Schema;

namespace SCADA.Modelos;

[Table("ImputacionMateriales")]
public class ImputacionMaterial
{
    public int Id { get; set; }
    public int IdOperacion { get; set; }
    public int IdMaterial { get; set; }
    public int IdEmpleado { get; set; }
    public decimal Cantidad { get; set; }
    public bool EsMerma { get; set; }
    public string? Observaciones { get; set; }
    public DateTime FechaRegistro { get; set; }

    [ForeignKey("IdOperacion")]
    public virtual OperacionesOrden OperacionesOrden{ get; set; }
    [ForeignKey("IdMaterial")]
    public virtual Material Material { get; set; }
    [ForeignKey("IdEmpleado")]
    public virtual Empleado Empleado { get; set; }

}
