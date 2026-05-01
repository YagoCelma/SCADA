using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCADA.Modelos
{
    [Table("Ordenes")]
    public class Orden
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string CodigoOrden { get; set; } = string.Empty;
        public string Producto {  get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; } = DateTime.Now;
        public DateTime? FechaFin { get; set; }
        public string Estado { get; set; } = "Pendiente";

        [NotMapped]
        public int PorcentajeTiempoTotal { get; set; }

        public virtual ICollection<OperacionesOrden> Operaciones { get; set; } = new List<OperacionesOrden>();

    }
}
