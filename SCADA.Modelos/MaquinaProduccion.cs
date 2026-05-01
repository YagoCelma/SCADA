using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCADA.Modelos;

[Table("MaquinaProduccion")]
public class MaquinaProduccion
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int MaquinaId { get; set; }

    [Required]
    public int CiclosReales { get; set; } = 500;

    public DateTime? FechaRegistro { get; set; }

    [ForeignKey(nameof(MaquinaId))]
    public Maquina? Maquina { get; set; }
}
