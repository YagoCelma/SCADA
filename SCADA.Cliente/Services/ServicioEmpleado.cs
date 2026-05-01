using SCADA.Modelos;
using System.Net.Http.Json;

namespace SCADA.Cliente.Services;

public class ServicioEmpleado
{
    private readonly HttpClient _http;

    public ServicioEmpleado(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Empleado>> GetAllAsync()
    {
        var resultado = await _http.GetFromJsonAsync<List<Empleado>>("api/empleados");
        return resultado ?? new List<Empleado>();
    }

    public async Task<List<Empleado>> ObtenerOperariosPorMaquinaAsync(int idMaquina)
    {
        var resultado = await _http.GetFromJsonAsync<List<Empleado>>($"api/empleados/pormaquina/{idMaquina}");
        return resultado ?? new List<Empleado>();
    }

    public async Task<List<Maquina>> ObtenerMaquinasPorEmpleadoAsync(int idEmpleado)
    {
        var resultado = await _http.GetFromJsonAsync<List<Maquina>>($"api/empleados/{idEmpleado}/maquinas");
        return resultado ?? new List<Maquina>();
    }

    public async Task AsignarEmpleadoAMaquinaAsync(int idMaquina, int idEmpleado)
    {
        await _http.PostAsync($"api/empleados/asignar/{idMaquina}/{idEmpleado}", null);
    }

    public async Task DesvincularEmpleadoDeMaquinaAsync(int idMaquina, int idEmpleado)
    {
        await _http.DeleteAsync($"api/empleados/desvincular/{idMaquina}/{idEmpleado}");
    }

    public async Task<List<Empleado>> GetAllEmpleadosAsync()
    {
        var result = await _http.GetFromJsonAsync<List<Empleado>>("api/empleados");
        return result ?? new List<Empleado>();
    }

    public async Task<bool> AddEmpleadoAsync(Empleado empleado)
    {
        var response = await _http.PostAsJsonAsync("api/empleados", empleado);
        return await response.Content.ReadFromJsonAsync<bool>();
    }

    public async Task UpdateEmpleadoAsync(Empleado empleado)
    {
        await _http.PutAsJsonAsync("api/empleados", empleado);
    }

    public async Task DeleteEmpleadoAsync(Empleado empleado)
    {
        await _http.DeleteAsync($"api/empleados/{empleado.Id}");
    }
    public async Task<List<Empleado>> GetAllEmpleadosConMaquinaAsync()
    {
        var result = await _http.GetFromJsonAsync<List<Empleado>>("api/empleados/empleadosconmaquina");
        return result ?? new List<Empleado>();
    }
}