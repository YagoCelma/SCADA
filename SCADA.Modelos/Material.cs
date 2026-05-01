using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCADA.Modelos;

[Table("Materiales")]
public class Material
{
    [Key]
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string CodigoMaterial { get; set; }
    public string? Descripcion { get; set; }
    public decimal Stock { get; set; }
    public decimal StockMinimo { get; set; }

    public virtual ICollection<MaquinaMaterial> MaquinasMateriales { get; set; } = new List<MaquinaMaterial>();

    [NotMapped]
    public string BusquedaCombinada => $"{Nombre} {CodigoMaterial}";
}
