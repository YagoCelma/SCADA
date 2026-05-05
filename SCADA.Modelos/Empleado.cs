using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCADA.Modelos
{
    public class Empleado
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string  Apellidos{ get; set; } = string.Empty;
        public string? CodigoEmpleado { get; set; }
        public bool EstaActivo { get; set; }
        public string Cargo { get; set; } = "Operario"; 
        //public int IdMaquina { get; set; }

        //[ForeignKey("IdMaquina")]
        //public virtual Maquina? Maquina { get; set; }

        public string BusquedaCombinada => $"{Nombre} - {CodigoEmpleado}";
        public virtual ICollection<EmpleadoMaquina> MaquinasEmpleados { get; set; } = new List<EmpleadoMaquina>();
        public string BusquedaRapida => $"{Nombre} {Apellidos} {CodigoEmpleado}";
    }
}
