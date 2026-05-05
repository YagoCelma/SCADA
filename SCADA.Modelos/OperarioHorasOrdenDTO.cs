using System;
using System.Collections.Generic;
using System.Text;

namespace SCADA.Modelos;

public class OperarioHorasOrdenDTO
{
    public int IdEmpleado { get; set; }
    public string NombreCompleto { get; set; }
    public string CodigoEmpleado { get; set; }
    public decimal HorasTotales { get; set; }
}
