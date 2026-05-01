namespace SCADA.Modelos
{
    public class AsignarTrabajador
    {
        public int MaquinaId { get; set; }
        public string NombreMaquina { get; set; } = string.Empty;
        public string NombreSeccion { get; set; } = string.Empty;
        public int? EmpleadoId { get; set; }
        public string Cargo { get; set; } = "Operario";
    }
}
