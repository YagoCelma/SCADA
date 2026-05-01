namespace SCADA.Cliente.Services;

public class ServicioProductividad
{
    // para calcular la productividad
    public decimal CalcularProductividadPorHora(int ciclosReales, int ciclosObjetivo)
    {
        if (ciclosObjetivo <= 0) 
        {
            return 0;
        }

        // Calculamos el porcentaje: (Reales / Objetivo) * 100
        decimal resultado = (decimal)ciclosReales / ciclosObjetivo * 100;
    
        return Math.Round(resultado, 2);
    }

    public double Calcular(int ciclosReales, int ciclosObjetivo) 
    {
        return (double)CalcularProductividadPorHora(ciclosReales, ciclosObjetivo);
    }
}