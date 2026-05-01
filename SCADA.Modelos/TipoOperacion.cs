using System.ComponentModel.DataAnnotations;

namespace SCADA.Modelos;

public class TipoOperacion
{
    [Key]
    public int Id { get; set; }
    public string Nombre { get; set; }
    public int Preferencia { get; set; }
    public int TiempoTeorico { get; set; }
}
