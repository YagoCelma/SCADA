using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SCADA.Api.Data;
using SCADA.Modelos;

namespace SCADA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaquinasController : ControllerBase
    {
        private readonly AppDbContext _db;

        public MaquinasController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _db.Maquinas
                .AsNoTracking()
                .Include(m => m.Seccion)
                .Include(m => m.MaquinasEmpleados)
                .ThenInclude(me => me.Empleado)
                .ToListAsync();
            return Ok(result);
        }

        [HttpGet("disponibles")]
        public async Task<IActionResult> GetAllMaquinasDisponiblesAsync()
        {
            var result = await _db.Maquinas
                .AsNoTracking()
                .Include(m => m.Seccion)
                .Include(m => m.MaquinasEmpleados)
                .ThenInclude(me => me.Empleado)
                .Where(m => m.EstadoActualId != 4)
                .ToListAsync();
            return Ok(result);
        }

        [HttpGet("porseccion")]
        public async Task<IActionResult> GetAllBySeccionAsync()
        {
            var result = await _db.Maquinas
                .Include(m => m.Seccion)
                .ToListAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _db.Maquinas.FirstOrDefaultAsync(m => m.Id == id);
            return Ok(result);
        }

        [HttpPost("guardar")]
        public async Task<IActionResult> GuardarMaquina([FromBody] Maquina maquina)
        {
            if (maquina.Id == 0)
            {
                maquina.FechaCreacion = DateTime.UtcNow;
                maquina.FechaActualizacion = DateTime.UtcNow;
                _db.Maquinas.Add(maquina);
                await _db.SaveChangesAsync();
            }
            else
            {
                var dbMaquina = await _db.Maquinas.FindAsync(maquina.Id);
                if (dbMaquina != null)
                {
                    dbMaquina.Nombre = maquina.Nombre;

                    if (maquina.IdSeccion.HasValue && maquina.IdSeccion > 0)
                    {
                        dbMaquina.IdSeccion = maquina.IdSeccion;
                    }

                    dbMaquina.CiclosReales = maquina.CiclosReales;
                    dbMaquina.EstadoActualId = maquina.EstadoActualId;
                    dbMaquina.FechaActualizacion = DateTime.UtcNow;
                    dbMaquina.CiclosObjetivo = maquina.CiclosObjetivo;

                    var opActiva = await _db.OperacionesOrden
                        .FirstOrDefaultAsync(o => o.IdMaquina == maquina.Id && o.Estado == "Activa");

                    if (opActiva != null)
                    {
                        opActiva.CiclosObjetivo = maquina.CiclosObjetivo;
                        opActiva.PiezasFabricadas = maquina.PiezasFabricadas;
                        opActiva.PiezasRotas = maquina.PiezasRotas;
                    }

                    await _db.SaveChangesAsync();
                }
            }
            return Ok();
        }

        [HttpGet("todas")]
        public async Task<IActionResult> GetAllMaquinas()
        {
            var result = await _db.Maquinas.ToListAsync();
            return Ok(result);
        }

        [HttpPost("ciclos/{maquinaId}/{ciclosReales}")]
        public async Task<IActionResult> AddCycleAsync(int maquinaId, int ciclosReales)
        {
            var log = new MaquinaProduccion
            {
                MaquinaId = maquinaId,
                CiclosReales = ciclosReales,
                FechaRegistro = DateTime.UtcNow
            };
            _db.Producciones.Add(log);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("ordenes")]
        public async Task<IActionResult> GetAllOrdenes()
        {
            try
            {
                var result = await _db.Ordenes.ToListAsync();
                return Ok(result);
            }
            catch
            {
                return Ok(new List<Orden>());
            }
        }

        [HttpPost("operaciones")]
        public async Task<IActionResult> InsertarOperacion([FromBody] OperacionesOrden nuevaOp)
        {
            try
            {
                _db.OperacionesOrden.Add(nuevaOp);
                var resultado = await _db.SaveChangesAsync();
                return Ok(resultado > 0);
            }
            catch
            {
                return Ok(false);
            }
        }

        [HttpPost("insertar")]
        public async Task<IActionResult> InsertarMaquina([FromBody] Maquina nuevaMaquina)
        {
            try
            {
                nuevaMaquina.FechaCreacion = DateTime.UtcNow;
                nuevaMaquina.FechaActualizacion = DateTime.UtcNow;
                _db.Maquinas.Add(nuevaMaquina);
                return Ok(await _db.SaveChangesAsync() > 0);
            }
            catch
            {
                return Ok(false);
            }
        }

        [HttpPut("actualizar")]
        public async Task<IActionResult> ActualizarMaquina([FromBody] Maquina maquinaEditada)
        {
            try
            {
                maquinaEditada.FechaActualizacion = DateTime.UtcNow;
                _db.Maquinas.Update(maquinaEditada);
                return Ok(await _db.SaveChangesAsync() > 0);
            }
            catch
            {
                return Ok(false);
            }
        }

        [HttpGet("{maquinaId}/operarios")]
        public async Task<IActionResult> ObtenerOperariosDeUnaMaquina(int maquinaId)
        {
            try
            {
                var result = await _db.EmpleadoMaquinas
                    .Where(me => me.IdMaquina == maquinaId)
                    .Include(me => me.Empleado)
                    .Select(me => me.Empleado)
                    .AsNoTracking()
                    .ToListAsync();
                return Ok(result);
            }
            catch
            {
                return Ok(new List<Empleado>());
            }
        }

        [HttpGet("secciones")]
        public async Task<IActionResult> ObtenerSecciones()
        {
            try
            {
                var result = await _db.Secciones.OrderBy(s => s.Nombre).ToListAsync();
                return Ok(result);
            }
            catch
            {
                return Ok(new List<Seccion>());
            }
        }

        [HttpGet("{maquinaId}/operacionactiva")]
        public async Task<IActionResult> ComprobarOperacionActiva(int maquinaId)
        {
            var result = await _db.OperacionesOrden.AnyAsync(op => op.IdMaquina == maquinaId && op.Estado == "Activa");
            return Ok(result);
        }

        [HttpGet("materiales")]
        public async Task<IActionResult> ObtenerTodosLosMaterialesAsync()
        {
            var result = await _db.Materiales.AsNoTracking().OrderBy(n => n.Nombre).ToListAsync();
            return Ok(result);
        }

        [HttpGet("{idMaquina}/materiales")]
        public async Task<IActionResult> ObtenerMaterialesDeMaquinaAsync(int idMaquina)
        {
            var materiales = await _db.MaquinasMateriales
                .AsNoTracking()
                .Where(mm => mm.IdMaquina == idMaquina)
                .Include(mm => mm.Material)
                .Select(mm => mm.Material)
                .ToListAsync();
            return Ok(materiales ?? new List<Material>());
        }

        [HttpPost("asignarmaterial/{idMaquina}/{idMaterial}")]
        public async Task<IActionResult> AsignarMaterialAMaquinaAsync(int idMaquina, int idMaterial)
        {
            var existe = await _db.MaquinasMateriales
                .AnyAsync(mm => mm.IdMaquina == idMaquina && mm.IdMaterial == idMaterial);

            if (!existe)
            {
                var nuevaRelacion = new MaquinaMaterial { IdMaquina = idMaquina, IdMaterial = idMaterial };
                _db.MaquinasMateriales.Add(nuevaRelacion);
                await _db.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpDelete("desvincularmaterial/{idMaquina}/{idMaterial}")]
        public async Task<IActionResult> DesvincularMaterialDeMaquinaAsync(int idMaquina, int idMaterial)
        {
            var relacion = await _db.MaquinasMateriales
                .FirstOrDefaultAsync(mm => mm.IdMaquina == idMaquina && mm.IdMaterial == idMaterial);

            if (relacion != null)
            {
                _db.MaquinasMateriales.Remove(relacion);
                await _db.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPost("asignarempleado/{idMaquina}/{idEmpleado}")]
        public async Task<IActionResult> AsignarEmpleadoAMaquinaAsync(int idMaquina, int idEmpleado)
        {
            var existe = await _db.EmpleadoMaquinas
                .AnyAsync(em => em.IdMaquina == idMaquina && em.IdEmpleado == idEmpleado);

            if (!existe)
            {
                var nuevaRelacion = new EmpleadoMaquina { IdMaquina = idMaquina, IdEmpleado = idEmpleado };
                _db.EmpleadoMaquinas.Add(nuevaRelacion);
                await _db.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpDelete("desvincularempleado/{idMaquina}/{idEmpleado}")]
        public async Task<IActionResult> DesvincularEmpleadoDeMaquinaAsync(int idMaquina, int idEmpleado)
        {
            var relacion = await _db.EmpleadoMaquinas
                .FirstOrDefaultAsync(em => em.IdMaquina == idMaquina && em.IdEmpleado == idEmpleado);

            if (relacion != null)
            {
                _db.EmpleadoMaquinas.Remove(relacion);
                await _db.SaveChangesAsync();
            }
            return Ok();
        }
    }
}