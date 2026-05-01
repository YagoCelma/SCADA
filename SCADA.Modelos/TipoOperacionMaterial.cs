using System.ComponentModel.DataAnnotations.Schema;

namespace SCADA.Modelos;

[Table("TiposOperacionesMateriles")]
public class TipoOperacionMaterial
{
    public int IdTipoOperacion { get; set; }
    public int IdMaterial { get; set; }

    [ForeignKey("IdTipoOperacion")]
    public virtual TipoOperacion TipoOperacion { get; set; } = null;
    [ForeignKey("IdMaterial")]
    public virtual Material Material { get; set; } = null;
}
