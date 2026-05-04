using System.Net.Http.Json;
using SCADA.Modelos;

namespace SCADA.Cliente.Services
{
    public class ImputacionService
    {
        private readonly HttpClient _http;

        public ImputacionService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Seccion>> GetSeccionesAsync()
        {
            return await _http.GetFromJsonAsync<List<Seccion>>("api/imputaciones/secciones") ?? new List<Seccion>();
        }

        public async Task<List<Maquina>> GetMaquinasBySeccionAsync(int idSeccion)
        {
            return await _http.GetFromJsonAsync<List<Maquina>>($"api/imputaciones/maquinas/{idSeccion}") ?? new List<Maquina>();
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            return await _http.GetFromJsonAsync<List<Empleado>>("api/imputaciones/empleados") ?? new List<Empleado>();
        }

        public async Task<List<Operacion>> GetOperacionesAsync()
        {
            return await _http.GetFromJsonAsync<List<Operacion>>("api/imputaciones/operaciones") ?? new List<Operacion>();
        }

        public async Task<List<Orden>> GetOrdenesActivasAsync()
        {
            return await _http.GetFromJsonAsync<List<Orden>>("api/imputaciones/ordenes/activas") ?? new List<Orden>();
        }

        public async Task<bool> GuardarImputacionAsync(ImputacionOperario imputacion)
        {
            var response = await _http.PostAsJsonAsync("api/imputaciones/imputacion/guardar", imputacion);
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> CrearNuevaOrdenAsync(Orden orden)
        {
            var response = await _http.PostAsJsonAsync("api/imputaciones/orden/nueva", orden);
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<string> GenerarProximoCodigoOrdenAsync()
        {
            return await _http.GetStringAsync("api/imputaciones/orden/proximo-codigo") ?? string.Empty;
        }

        public async Task<bool> InsertarOrdenMadreAsync(Orden orden)
        {
            var response = await _http.PostAsJsonAsync("api/imputaciones/orden/madre", orden);
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<List<Orden>> ObtenerOrdenesActivasParaAsignarAsync()
        {
            return await _http.GetFromJsonAsync<List<Orden>>("api/imputaciones/orden/para-asignar") ?? new List<Orden>();
        }

        public async Task<bool> CrearNuevaOperacionAsync(int idOrden, int idMaquina, int ciclos)
        {
            var response = await _http.PostAsync($"api/imputaciones/operacion/nueva/{idOrden}/{idMaquina}/{ciclos}", null);
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<Orden?> GetOrdenById(string codigo)
        {
            return await _http.GetFromJsonAsync<Orden>($"api/imputaciones/orden/codigo/{codigo}");
        }

        public async Task<bool> AsignarOrdenAMaquinaAsync(int idOrden, int idMaquina, int ciclos)
        {
            var response = await _http.PostAsync($"api/imputaciones/orden/asignar/{idOrden}/{idMaquina}/{ciclos}", null);
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<List<Orden>> ObtenerOrdenesActivasAsync()
        {
            return await _http.GetFromJsonAsync<List<Orden>>("api/imputaciones/orden/encurso") ?? new List<Orden>();
        }

        public async Task<List<Orden>> ObtenerOrdenesAsync()
        {
            return await _http.GetFromJsonAsync<List<Orden>>("api/imputaciones/orden/todas") ?? new List<Orden>();
        }

        public async Task<List<OperacionResumenDTO>> ObtenerOperacionesActivasAsync()
        {
            return await _http.GetFromJsonAsync<List<OperacionResumenDTO>>("api/imputaciones/operacion/resumen-activas") ?? new List<OperacionResumenDTO>();
        }

        public async Task<string> GenerarSiguienteCodigoOperacionAsync(string codigoOrdenBase)
        {
            return await _http.GetStringAsync($"api/imputaciones/operacion/proximo-codigo/{codigoOrdenBase}") ?? string.Empty;
        }

        public async Task<List<OperacionResumenDTO>> ObtenerTodasLasOperacionesResumenAsync()
        {
            return await _http.GetFromJsonAsync<List<OperacionResumenDTO>>("api/imputaciones/operacion/resumen-todas") ?? new List<OperacionResumenDTO>();
        }

        public async Task<bool> ActualizarOrdenAsync(Orden orden)
        {
            var response = await _http.PutAsJsonAsync("api/imputaciones/orden/actualizar", orden);
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> InsertarImputacionOperario(ImputacionOperario nuevaImp)
        {
            var response = await _http.PostAsJsonAsync("api/imputaciones/imputacionoperario", nuevaImp);
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task ActualizarCierreOperacion(dynamic operacion)
        {
            var req = new
            {
                Id = (int)operacion.Id,
                IdMaquina = (int)operacion.IdMaquina,
                FechaFin = (DateTime)operacion.FechaFin,
                PiezasFabricadas = (int)operacion.PiezasFabricadas,
                PiezasRotas = (int)operacion.PiezasRotas
            };

            await _http.PutAsJsonAsync("api/imputaciones/operacion/cierre-dinamico", req);
        }

        public async Task<List<Material>> ObtenerMaterialesAsync()
        {
            return await _http.GetFromJsonAsync<List<Material>>("api/imputaciones/materiales") ?? new List<Material>();
        }

        public async Task<bool> GuardarSeccionAsync(Seccion nuevaSeccion)
        {
            var response = await _http.PostAsJsonAsync("api/imputaciones/seccion/guardar", nuevaSeccion);
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> GuardarMaterialAsync(Material material)
        {
            var response = await _http.PostAsJsonAsync("api/imputaciones/material/guardar", material);
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> GuardarMultiplesMaterialesAsync(List<ImputacionMaterial> lista)
        {
            var response = await _http.PostAsJsonAsync("api/imputaciones/material/guardar-multiples", lista);
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<List<OperarioHorasDTO>> ObtenerHorasPorOperarioAsync(int idOperacion)
        {
            return await _http.GetFromJsonAsync<List<OperarioHorasDTO>>($"api/imputaciones/operacion/{idOperacion}/horas-operario") ?? new List<OperarioHorasDTO>();
        }

        public async Task<string> GenerarCodigoMaterial(string nombreMaterial)
        {
            return await _http.GetStringAsync($"api/imputaciones/material/generar-codigo/{nombreMaterial}") ?? string.Empty;
        }

        public async Task<bool> ImputarTrabajoDesdeTerminalAsync(int idOperacion, int idMaquina, int idEmpleado, DateTime inicio, DateTime fin, int piezasHechasTurno, int piezasRotasTurno)
        {
            var req = new
            {
                IdOperacion = idOperacion,
                IdMaquina = idMaquina,
                IdEmpleado = idEmpleado,
                Inicio = inicio,
                Fin = fin,
                PiezasHechasTurno = piezasHechasTurno,
                PiezasRotasTurno = piezasRotasTurno
            };

            var response = await _http.PostAsJsonAsync("api/imputaciones/fichaje/terminal", req);
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> CerrarOperacionAsync(int idOperacion, int idMaquina, int? idSeccion)
        {
            int seccion = idSeccion ?? 0;
            var response = await _http.PutAsync($"api/imputaciones/operacion/cerrar/{idOperacion}/{idMaquina}/{seccion}", null);
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> VerificarOperacionCerrada(int idOperacion)
        {
            return await _http.GetFromJsonAsync<bool>($"api/imputaciones/operacion/verificar-cerrada/{idOperacion}");
        }

        public async Task<List<Material>> ObtenerMaterialesPorMaquinaAsync(int idMaquina)
        {
            return await _http.GetFromJsonAsync<List<Material>>($"api/imputaciones/maquina/{idMaquina}/materiales") ?? new List<Material>();
        }

        public async Task<bool> RegistrarConsumoMaterialAsync(int idOperacion, int idMaterial, decimal cantidad, int idEmpleado, string? observaciones = null)
        {
            var req = new
            {
                IdOperacion = idOperacion,
                IdMaterial = idMaterial,
                Cantidad = cantidad,
                IdEmpleado = idEmpleado,
                Observaciones = observaciones
            };

            var response = await _http.PostAsJsonAsync("api/imputaciones/material/consumo-normal", req);
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<List<ImputacionMaterial>> ObtenerConsumosPorOperacionAsync(int idOperacion)
        {
            return await _http.GetFromJsonAsync<List<ImputacionMaterial>>($"api/imputaciones/operacion/{idOperacion}/consumos") ?? new List<ImputacionMaterial>();
        }

        public async Task<bool> RestadoStockAsync(int idMaterial, decimal cantidad)
        {
            var response = await _http.PutAsync($"api/imputaciones/material/restar-stock/{idMaterial}/{cantidad}", null);
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> StockMinimoAsync(int idMaquina, decimal cantidad)
        {
            return await _http.GetFromJsonAsync<bool>($"api/imputaciones/material/stock-minimo/{idMaquina}/{cantidad}");
        }

        public async Task<bool> EliminarConsumoAsync(int idConsumo)
        {
            var response = await _http.DeleteAsync($"api/imputaciones/material/consumo/{idConsumo}");
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> RegistrarConsumoMaterialAsync(int idOperacion, int idMaterial, decimal cantidad, int idEmpleado, bool esMerma)
        {
            var req = new
            {
                IdOperacion = idOperacion,
                IdMaterial = idMaterial,
                Cantidad = cantidad,
                IdEmpleado = idEmpleado,
                EsMerma = esMerma
            };

            var response = await _http.PostAsJsonAsync("api/imputaciones/material/consumo-merma", req);
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> PuedeIniciarOperacionAsync(int idOperacion, int idOrden)
        {
            return await _http.GetFromJsonAsync<bool>($"api/imputaciones/operacion/puede-iniciar/{idOperacion}/{idOrden}");
        }

        public async Task IniciarFichajeAsync(int idOperacion, int idEmpleado)
        {
            await _http.PostAsync($"api/imputaciones/fichaje/iniciar-basico/{idOperacion}/{idEmpleado}", null);
        }

        public async Task GenerarHojaRutaAsync(int idOrden)
        {
            await _http.PostAsync($"api/imputaciones/orden/{idOrden}/hoja-ruta", null);
        }

        public async Task<List<TipoOperacion>> ObtenerOperacionesActivas()
        {
            return await _http.GetFromJsonAsync<List<TipoOperacion>>("api/imputaciones/tipos-operaciones") ?? new List<TipoOperacion>();
        }

        public async Task<bool> VincularMaquinaAOperacionAsync(int idOperacion, int idMaquina)
        {
            var response = await _http.PutAsync($"api/imputaciones/operacion/vincular/{idOperacion}/{idMaquina}", null);
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> LiberarMaquinaYOperacionAsync(int idOperacion, int idMaquina)
        {
            var response = await _http.PutAsync($"api/imputaciones/operacion/liberar/{idOperacion}/{idMaquina}", null);
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<List<Material>> ObtenerMaterialesPermitidosAsync(int idOperacionDiaria)
        {
            return await _http.GetFromJsonAsync<List<Material>>($"api/imputaciones/operacion/{idOperacionDiaria}/materiales-permitidos") ?? new List<Material>();
        }

        public async Task<List<ImputacionMaterial>> ObtenerImputacionMaterialesAsync(int idOperacion)
        {
            return await _http.GetFromJsonAsync<List<ImputacionMaterial>>($"api/imputaciones/operacion/{idOperacion}/imputaciones") ?? new List<ImputacionMaterial>();
        }

        public async Task<List<ImputacionMaterial>> ObtenerMaterialesPorOrdenAsync(int idOrden)
        {
            return await _http.GetFromJsonAsync<List<ImputacionMaterial>>($"api/imputaciones/orden/{idOrden}/materiales-consumidos") ?? new List<ImputacionMaterial>();
        }

        public async Task<List<OperacionesOrden>> ObtenerOperacionesOrdenAsync(int idOrden)
        {
            return await _http.GetFromJsonAsync<List<OperacionesOrden>>($"api/imputaciones/orden/{idOrden}/operaciones-detalle") ?? new List<OperacionesOrden>();
        }

        public async Task IniciarOReanudarFichajeAsync(int idOperacion, int idEmpleado)
        {
            await _http.PostAsync($"api/imputaciones/fichaje/iniciar-reanudar/{idOperacion}/{idEmpleado}", null);
        }

        public async Task PausarFichajeAsync(int idOperacion, int idEmpleado)
        {
            await _http.PutAsync($"api/imputaciones/fichaje/pausar/{idOperacion}/{idEmpleado}", null);
        }

        public async Task FinalizarOperacionAsync(int idOperacion, int idEmpleado)
        {
            await _http.PutAsync($"api/imputaciones/fichaje/finalizar/{idOperacion}/{idEmpleado}", null);
        }

        public async Task<List<Orden>> ObtenerOrdenesConProgresoAsync()
        {
            return await _http.GetFromJsonAsync<List<Orden>>("api/imputaciones/orden/progreso") ?? new List<Orden>();
        }

        public async Task <bool> EliminarFichajeAbiertoAsync(int idOperacion, int idEmpleado)
        {
            var respuesta = await _http.DeleteAsync($"api/imputaciones/fichaje/abierto/{idOperacion}/{idEmpleado}");
            return await respuesta.Content.ReadFromJsonAsync<bool>();
        }
    }
}