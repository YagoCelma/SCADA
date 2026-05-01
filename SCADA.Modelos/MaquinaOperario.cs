using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCADA.Modelos;

[Table("MaquinasOperarios")]
public class MaquinaOperario
{
    [Key]
    public int Id { get; set; }
    public int MaquinaId { get; set; }
    [ForeignKey("MaquinaId")]
    public virtual Maquina? Maquina { get; set; }

    public int EmpleadoId { get; set; }
    [ForeignKey("EmpleadoId")]
    public virtual Empleado? Empleado { get; set; }

    public DateTime FechaAsignacion { get; set; }
}
