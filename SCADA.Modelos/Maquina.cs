using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace SCADA.Modelos
{
    [Table("Maquina")]
    public class Maquina
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int? IdSeccion { get; set; }
        public int CiclosReales { get; set; }
        //public string? NombreSeccion { get; set; } = string.Empty;
        //public int? EmpleadoId { get; set; }
        public int? EstadoActualId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        [NotMapped]
        public string NombreOperario { get; set; } = string.Empty ;
        [NotMapped]
        public string CargoOperario { get; set; } = string.Empty;
        [NotMapped]
        public float PorcentajeCiclos { get; set;  }
        [NotMapped]
        public int CiclosObjetivo { get; set; }
        [NotMapped]
        public string? Operacion { get; set; }
        [NotMapped]
        public int PiezasFabricadas { get; set; }
        [NotMapped]
        public int PiezasRotas { get; set; }
        [NotMapped]
        public string CodigoOperacionActiva { get; set; } = "Sin operacion";

        [ForeignKey("IdSeccion")]
        public virtual Seccion? Seccion { get; set; }

        public virtual ICollection<MaquinaProduccion> Producciones { get; set; } = new List<MaquinaProduccion>();

        [ForeignKey("EstadoActualId")]
        public virtual MaquinaEstatus? EstadoActual { get; set; }

        public virtual ICollection<MaquinaMaterial> MaquinasMateriales { get; set; } = new List<MaquinaMaterial>();
        public virtual ICollection<EmpleadoMaquina> MaquinasEmpleados { get; set; } = new List<EmpleadoMaquina>();
    }
}