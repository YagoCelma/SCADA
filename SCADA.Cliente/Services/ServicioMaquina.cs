using System.Net.Http.Json;
using SCADA.Modelos; // Importante: Usando tu namespace correcto

namespace SCADA.Cliente.Services
{
    public class ServicioMaquina
    {
        private readonly HttpClient _http;

        public ServicioMaquina(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Maquina>> GetAllAsync()
        {
            var result = await _http.GetFromJsonAsync<List<Maquina>>("api/maquinas");
            return result ?? new List<Maquina>();
        }

        public async Task<List<Maquina>> GetAllMaquinasDisponiblesAsync()
        {
            var result = await _http.GetFromJsonAsync<List<Maquina>>("api/maquinas/disponibles");
            return result ?? new List<Maquina>();
        }

        public async Task<List<Maquina>> GetAllBySeccionAsync()
        {
            var result = await _http.GetFromJsonAsync<List<Maquina>>("api/maquinas/porseccion");
            return result ?? new List<Maquina>();
        }

        public async Task<Maquina?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<Maquina>($"api/maquinas/{id}");
        }

        public async Task GuardarMaquina(Maquina maquina)
        {
            await _http.PostAsJsonAsync("api/maquinas/guardar", maquina);
        }

        public async Task<List<Maquina>> GetAllMaquinas()
        {
            var result = await _http.GetFromJsonAsync<List<Maquina>>("api/maquinas/todas");
            return result ?? new List<Maquina>();
        }

        public async Task AddCycleAsync(int maquinaId, int ciclosReales)
        {
            await _http.PostAsync($"api/maquinas/ciclos/{maquinaId}/{ciclosReales}", null);
        }

        public async Task<List<Orden>> GetAllOrdenes()
        {
            var result = await _http.GetFromJsonAsync<List<Orden>>("api/maquinas/ordenes");
            return result ?? new List<Orden>();
        }

        public async Task<bool> InsertarOperacion(OperacionesOrden nuevaOp)
        {
            var response = await _http.PostAsJsonAsync("api/maquinas/operaciones", nuevaOp);
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> InsertarMaquina(Maquina nuevaMaquina)
        {
            var response = await _http.PostAsJsonAsync("api/maquinas/insertar", nuevaMaquina);
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> ActualizarMaquina(Maquina maquinaEditada)
        {
            var response = await _http.PutAsJsonAsync("api/maquinas/actualizar", maquinaEditada);
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<List<Empleado>> ObtenerOperariosDeUnaMaquina(int maquinaId)
        {
            var result = await _http.GetFromJsonAsync<List<Empleado>>($"api/maquinas/{maquinaId}/operarios");
            return result ?? new List<Empleado>();
        }

        public async Task<List<Seccion>> ObtenerSecciones()
        {
            var result = await _http.GetFromJsonAsync<List<Seccion>>("api/maquinas/secciones");
            return result ?? new List<Seccion>();
        }

        public async Task<bool> ComprobarOperacionActiva(int maquinaId)
        {
            return await _http.GetFromJsonAsync<bool>($"api/maquinas/{maquinaId}/operacionactiva");
        }

        public async Task<List<Material>> ObtenerTodosLosMaterialesAsync()
        {
            var result = await _http.GetFromJsonAsync<List<Material>>("api/maquinas/materiales");
            return result ?? new List<Material>();
        }

        public async Task<List<Material>> ObtenerMaterialesDeMaquinaAsync(int idMaquina)
        {
            var result = await _http.GetFromJsonAsync<List<Material>>($"api/maquinas/{idMaquina}/materiales");
            return result ?? new List<Material>();
        }

        public async Task AsignarMaterialAMaquinaAsync(int idMaquina, int idMaterial)
        {
            await _http.PostAsync($"api/maquinas/asignarmaterial/{idMaquina}/{idMaterial}", null);
        }

        public async Task DesvincularMaterialDeMaquinaAsync(int idMaquina, int idMaterial)
        {
            await _http.DeleteAsync($"api/maquinas/desvincularmaterial/{idMaquina}/{idMaterial}");
        }

        public async Task AsignarEmpleadoAMaquinaAsync(int idMaquina, int idEmpleado)
        {
            await _http.PostAsync($"api/maquinas/asignarempleado/{idMaquina}/{idEmpleado}", null);
        }

        public async Task DesvincularEmpleadoDeMaquinaAsync(int idMaquina, int idEmpleado)
        {
            await _http.DeleteAsync($"api/maquinas/desvincularempleado/{idMaquina}/{idEmpleado}");
        }
    }
}