using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SCADA.Api.Data;
using SCADA.Modelos;

namespace SCADA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly AppDbContext _db;

        public EmpleadosController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var empleados = await _db.Empleados.AsNoTracking().ToListAsync();
            return Ok(empleados);
        }

        [HttpGet("pormaquina/{idMaquina}")]
        public async Task<IActionResult> ObtenerOperariosPorMaquinaAsync(int idMaquina)
        {
            var empleados = await _db.EmpleadoMaquinas
                .Where(me => me.IdMaquina == idMaquina)
                .Include(me => me.Empleado)
                .Select(me => me.Empleado)
                .ToListAsync();

            return Ok(empleados ?? new List<Empleado>());
        }

        [HttpGet("{idEmpleado}/maquinas")]
        public async Task<IActionResult> ObtenerMaquinasPorEmpleadoAsync(int idEmpleado)
        {
            var maquinas = await _db.EmpleadoMaquinas
                .Where(me => me.IdEmpleado == idEmpleado)
                .Include(me => me.Maquina)
                .Select(me => me.Maquina)
                .ToListAsync();

            return Ok(maquinas ?? new List<Maquina>());
        }

        [HttpPost("asignar/{idMaquina}/{idEmpleado}")]
        public async Task<IActionResult> AsignarEmpleadoAMaquinaAsync(int idMaquina, int idEmpleado)
        {
            var existe = await _db.EmpleadoMaquinas
                .AnyAsync(me => me.IdMaquina == idMaquina && me.IdEmpleado == idEmpleado);

            if (!existe)
            {
                var nuevaRelacion = new EmpleadoMaquina
                {
                    IdMaquina = idMaquina,
                    IdEmpleado = idEmpleado
                };
                _db.EmpleadoMaquinas.Add(nuevaRelacion);
                await _db.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpDelete("desvincular/{idMaquina}/{idEmpleado}")]
        public async Task<IActionResult> DesvincularEmpleadoDeMaquinaAsync(int idMaquina, int idEmpleado)
        {
            var relacion = await _db.EmpleadoMaquinas
                .FirstOrDefaultAsync(me => me.IdMaquina == idMaquina && me.IdEmpleado == idEmpleado);

            if (relacion != null)
            {
                _db.EmpleadoMaquinas.Remove(relacion);
                await _db.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddEmpleadoAsync([FromBody] Empleado empleado)
        {
            try
            {
                _db.Empleados.Add(empleado);
                await _db.SaveChangesAsync();
                return Ok(true);
            }
            catch (DbUpdateException)
            {
                _db.Entry(empleado).State = EntityState.Detached;
                return Ok(false);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEmpleadoAsync([FromBody] Empleado empleado)
        {
            var dbEmpleado = await _db.Empleados.FindAsync(empleado.Id);
            if (dbEmpleado != null)
            {
                dbEmpleado.Nombre = empleado.Nombre;
                dbEmpleado.Apellidos = empleado.Apellidos;
                dbEmpleado.Cargo = empleado.Cargo;
                await _db.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmpleadoAsync(int id)
        {
            var dbEmpleado = await _db.Empleados.FindAsync(id);
            if (dbEmpleado != null)
            {
                _db.Empleados.Remove(dbEmpleado);
                await _db.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpGet("empleadosconmaquina")]
        public async Task<IActionResult> GetAllEmpleadosConMaquinaAsync()
        {
            try
            {
                var result = await _db.Empleados
                    .Include(e => e.MaquinasEmpleados)
                        .ThenInclude(me => me.Maquina)
                    .AsNoTracking()
                    .ToListAsync();
                return Ok(result);
            }
            catch
            {
                return Ok(new List<Empleado>());
            }
        }
    }
}