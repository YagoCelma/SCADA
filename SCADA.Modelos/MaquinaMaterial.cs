using System.ComponentModel.DataAnnotations.Schema;

namespace SCADA.Modelos;

[Table("MaquinasMateriales")]
public class MaquinaMaterial
{
    public int IdMaquina { get; set; }
    public int IdMaterial { get; set; }

    [ForeignKey("IdMaquina")]
    public virtual Maquina Maquina { get; set; } = null;

    [ForeignKey("IdMaterial")]
    public virtual Material Material { get; set; } = null;
    
}
