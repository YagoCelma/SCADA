using System.ComponentModel.DataAnnotations.Schema;

namespace SCADA.Modelos
{
    public class OperacionesOrden
    {
        public int Id { get; set; }
        public int IdOrden { get; set; }
        public string CodigoOperacion { get; set; } = string.Empty;
        public int? IdSeccion { get; set; }
        public int? IdMaquina { get; set; }
        public int IdOperacionMaestra { get; set; }
        public int Preferencia { get; set; }
        public int CiclosObjetivo { get; set; }
        public int PiezasFabricadas { get; set; }
        public int PiezasRotas { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Estado { get; set; } = "Pendiente";

        [ForeignKey("IdOrden")]
        public virtual Orden? Orden { get; set; }

        [ForeignKey("IdSeccion")]
        public virtual Seccion? Seccion { get; set; }

        [ForeignKey("IdMaquina")]
        public virtual Maquina? Maquina { get; set; }

        [ForeignKey("IdOperacionMaestra")]
        public virtual TipoOperacion? DetalleOperacion { get; set; }
        [InverseProperty("Operacion")]
        public virtual ICollection<ImputacionOperario> Imputaciones { get; set; } = new List<ImputacionOperario>();
    }
}
