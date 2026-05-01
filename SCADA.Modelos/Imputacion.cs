using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCADA.Modelos;

[Table("Imputaciones")]
public class Imputacion
{
    [Key]
    private int Id { get; set; }

}
