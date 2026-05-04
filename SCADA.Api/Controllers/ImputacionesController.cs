using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SCADA.Api.Data;
using SCADA.Modelos;

namespace SCADA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImputacionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ImputacionesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("secciones")]
        public async Task<IActionResult> GetSeccionesAsync()
        {
            var secciones = await _context.Secciones
                .OrderBy(s => s.Nombre)
                .ToListAsync();

            return Ok(secciones);
        }

        [HttpGet("maquinas/{idSeccion}")]
        public async Task<IActionResult> GetMaquinasBySeccionAsync(int idSeccion)
        {
            var maquinas = await _context.Maquinas
                .Where(m => m.IdSeccion == idSeccion)
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            return Ok(maquinas);
        }

        [HttpGet("empleados")]
        public async Task<IActionResult> GetEmpleadosAsync()
        {
            var empleados = await _context.Empleados
                .OrderBy(e => e.Nombre)
                .ToListAsync();

            return Ok(empleados);
        }

        [HttpGet("operaciones")]
        public async Task<IActionResult> GetOperacionesAsync()
        {
            var operaciones = await _context.Operaciones
                .OrderBy(o => o.Nombre)
                .ToListAsync();

            return Ok(operaciones);
        }

        [HttpGet("ordenes/activas")]
        public async Task<IActionResult> GetOrdenesActivasAsync()
        {
            var ordenes = await _context.Ordenes.ToListAsync();
            return Ok(ordenes);
        }

        [HttpPost("imputacion/guardar")]
        public async Task<IActionResult> GuardarImputacionAsync([FromBody] ImputacionOperario imputacion)
        {
            try
            {
                _context.ImputacionesOperarios.Add(imputacion);
                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch
            {
                return Ok(false);
            }
        }

        [HttpPost("orden/nueva")]
        public async Task<IActionResult> CrearNuevaOrdenAsync([FromBody] Orden orden)
        {
            try
            {
                _context.Ordenes.Add(orden);
                await _context.SaveChangesAsync();
                await GenerarHojaRutaInt(orden.Id);
                return Ok(true);
            }
            catch
            {
                return Ok(false);
            }
        }

        [HttpGet("orden/proximo-codigo")]
        public async Task<IActionResult> GenerarProximoCodigoOrdenAsync()
        {
            string prefijo = $"ORD-{DateTime.Now:yyMM}-";
            var conteo = await _context.Ordenes.CountAsync(o => o.CodigoOrden.StartsWith(prefijo));

            return Ok($"{prefijo}{(conteo + 1):D3}");
        }

        [HttpPost("orden/madre")]
        public async Task<IActionResult> InsertarOrdenMadreAsync([FromBody] Orden orden)
        {
            try
            {
                _context.Ordenes.Add(orden);
                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch
            {
                return Ok(false);
            }
        }

        [HttpGet("orden/para-asignar")]
        public async Task<IActionResult> ObtenerOrdenesActivasParaAsignarAsync()
        {
            var ordenes = await _context.Ordenes
                .Where(o => o.Estado != "Cerrada")
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            return Ok(ordenes);
        }

        [HttpPost("operacion/nueva/{idOrden}/{idMaquina}/{ciclos}")]
        public async Task<IActionResult> CrearNuevaOperacionAsync(int idOrden, int idMaquina, int ciclos)
        {
            try
            {
                var ordenPadre = await _context.Ordenes.FindAsync(idOrden);
                var total = await _context.OperacionesOrden.CountAsync(op => op.IdOrden == idOrden);

                var nuevaOp = new OperacionesOrden
                {
                    IdOrden = idOrden,
                    CodigoOperacion = $"{ordenPadre.CodigoOrden}-{total + 1}",
                    IdMaquina = idMaquina,
                    CiclosObjetivo = ciclos,
                    Estado = "En curso",
                    FechaInicio = null
                };

                _context.OperacionesOrden.Add(nuevaOp);

                var maquina = await _context.Maquinas.FindAsync(idMaquina);
                if (maquina != null)
                {
                    maquina.EstadoActualId = 1;
                }

                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch
            {
                return Ok(false);
            }
        }

        [HttpGet("orden/codigo/{codigo}")]
        public async Task<IActionResult> GetOrdenById(string codigo)
        {
            var orden = await _context.Ordenes.FirstOrDefaultAsync(o => o.CodigoOrden == codigo);
            return Ok(orden);
        }

        [HttpPost("orden/asignar/{idOrden}/{idMaquina}/{ciclos}")]
        public async Task<IActionResult> AsignarOrdenAMaquinaAsync(int idOrden, int idMaquina, int ciclos)
        {
            var ordenPadre = await _context.Ordenes.FindAsync(idOrden);
            if (ordenPadre == null) return Ok(false);

            var conteo = await _context.OperacionesOrden.CountAsync(op => op.IdOrden == idOrden);

            var nuevaOp = new OperacionesOrden
            {
                IdOrden = idOrden,
                IdMaquina = idMaquina,
                CodigoOperacion = $"{ordenPadre.CodigoOrden}-{conteo + 1}",
                CiclosObjetivo = ciclos,
                PiezasFabricadas = 0,
                PiezasRotas = 0,
                Estado = "En curso",
                FechaInicio = null,
                IdSeccion = 1,
                IdOperacionMaestra = 1
            };

            _context.OperacionesOrden.Add(nuevaOp);

            var maquina = await _context.Maquinas.FindAsync(idMaquina);
            if (maquina != null)
            {
                maquina.EstadoActualId = 1;
            }

            return Ok(await _context.SaveChangesAsync() > 0);
        }

        [HttpGet("orden/encurso")]
        public async Task<IActionResult> ObtenerOrdenesActivasAsync2()
        {
            var ordenes = await _context.Ordenes
                .Where(o => o.Estado == "En curso")
                .OrderByDescending(o => o.FechaInicio)
                .ToListAsync();

            return Ok(ordenes);
        }

        [HttpGet("orden/todas")]
        public async Task<IActionResult> ObtenerOrdenesAsync()
        {
            var ordenes = await _context.Ordenes.ToListAsync();
            return Ok(ordenes);
        }

        [HttpGet("operacion/resumen-activas")]
        public async Task<IActionResult> ObtenerOperacionesActivasAsync()
        {
            var result = await _context.OperacionesOrden
                .Include(o => o.Orden)
                .Include(o => o.Maquina)
                .Where(o => (o.Estado == "En curso" || o.Estado == "Pendiente" || o.Estado == "Finalizado") && o.Orden.Estado != "Finalizado")
                .Select(o => new OperacionResumenDTO
                {
                    Id = o.Id,
                    IdMaquina = o.IdMaquina,
                    CodigoOperacion = o.CodigoOperacion,
                    NombreOrden = o.Orden != null ? o.Orden.CodigoOrden : "Sin Código",
                    Producto = o.Orden != null ? o.Orden.Producto : "Sin Producto",
                    NombreMaquina = o.Maquina != null ? o.Maquina.Nombre : "Sin Máquina",
                    CiclosObjetivo = o.CiclosObjetivo,
                    PiezasFabricadas = o.PiezasFabricadas,
                    PiezasRotas = o.PiezasRotas,
                    Estado = o.Estado,
                    FechaInicio = o.FechaInicio,
                    FechaFin = o.FechaFin
                })
                .OrderByDescending(o => o.FechaInicio)
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("operacion/proximo-codigo/{codigoOrdenBase}")]
        public async Task<IActionResult> GenerarSiguienteCodigoOperacionAsync(string codigoOrdenBase)
        {
            var cantidad = await _context.OperacionesOrden.CountAsync(o => o.CodigoOperacion.StartsWith(codigoOrdenBase));
            return Ok($"{codigoOrdenBase}-{cantidad + 1}");
        }

        [HttpGet("operacion/resumen-todas")]
        public async Task<IActionResult> ObtenerTodasLasOperacionesResumenAsync()
        {
            var result = await _context.OperacionesOrden
                .Include(o => o.Orden)
                .Include(o => o.Maquina)
                .Select(o => new OperacionResumenDTO
                {
                    Id = o.Id,
                    IdMaquina = o.IdMaquina,
                    CodigoOperacion = o.CodigoOperacion,
                    NombreOrden = o.Orden != null ? o.Orden.CodigoOrden : "Sin Orden",
                    Producto = o.Orden != null ? o.Orden.Producto : "Sin Producto",
                    NombreMaquina = o.Maquina != null ? o.Maquina.Nombre : "Sin Máquina",
                    CiclosObjetivo = o.CiclosObjetivo,
                    PiezasFabricadas = o.PiezasFabricadas,
                    PiezasRotas = o.PiezasRotas,
                    Estado = o.Estado,
                    FechaInicio = o.FechaInicio,
                    FechaFin = o.FechaFin
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpPut("orden/actualizar")]
        public async Task<IActionResult> ActualizarOrdenAsync([FromBody] Orden orden)
        {
            try
            {
                _context.Ordenes.Update(orden);
                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch
            {
                return Ok(false);
            }
        }

        [HttpPost("imputacionoperario")]
        public async Task<IActionResult> InsertarImputacionOperario([FromBody] ImputacionOperario nuevaImp)
        {
            try
            {
                nuevaImp.Operacion = null;
                nuevaImp.Empleado = null;
                _context.ImputacionesOperarios.Add(nuevaImp);
                return Ok(await _context.SaveChangesAsync() > 0);
            }
            catch
            {
                return Ok(false);
            }
        }

        [HttpPut("operacion/cierre-dinamico")]
        public async Task<IActionResult> ActualizarCierreOperacion([FromBody] CierreOperacionRequest req)
        {
            var op = await _context.OperacionesOrden.FindAsync(req.Id);
            if (op != null)
            {
                op.Estado = "Finalizado";
                op.FechaFin = req.FechaFin;
                op.PiezasFabricadas = req.PiezasFabricadas;
                op.PiezasRotas = req.PiezasRotas;

                if (!await _context.OperacionesOrden.AnyAsync(o => o.IdOrden == op.IdOrden && o.Estado != "Finalizado"))
                {
                    var ordenMadre = await _context.Ordenes.FindAsync(op.IdOrden);
                    if (ordenMadre != null)
                    {
                        ordenMadre.Estado = "Finalizado";
                    }
                }
            }

            var maquina = await _context.Maquinas.FindAsync(req.IdMaquina);
            if (maquina != null)
            {
                if (maquina.EstadoActualId != 4)
                {
                    maquina.EstadoActualId = 3;
                }
                maquina.CiclosReales = 0;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("materiales")]
        public async Task<IActionResult> ObtenerMaterialesAsync()
        {
            var materiales = await _context.Materiales.OrderByDescending(e => e.Nombre).ToListAsync();
            return Ok(materiales);
        }

        [HttpPost("seccion/guardar")]
        public async Task<IActionResult> GuardarSeccionAsync([FromBody] Seccion nuevaSeccion)
        {
            try
            {
                _context.Secciones.Add(nuevaSeccion);
                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch
            {
                return Ok(false);
            }
        }

        [HttpPost("material/guardar")]
        public async Task<IActionResult> GuardarMaterialAsync([FromBody] Material material)
        {
            try
            {
                if (material.Id == 0)
                {
                    _context.Materiales.Add(material);
                }
                else
                {
                    _context.Materiales.Update(material);
                }

                return Ok(await _context.SaveChangesAsync() > 0);
            }
            catch
            {
                return Ok(false);
            }
        }

        [HttpPost("material/guardar-multiples")]
        public async Task<IActionResult> GuardarMultiplesMaterialesAsync([FromBody] List<ImputacionMaterial> lista)
        {
            try
            {
                _context.ImputacionMateriales.AddRange(lista);
                return Ok(await _context.SaveChangesAsync() > 0);
            }
            catch
            {
                return Ok(false);
            }
        }

        [HttpGet("operacion/{idOperacion}/horas-operario")]
        public async Task<IActionResult> ObtenerHorasPorOperarioAsync(int idOperacion)
        {
            var result = await _context.ImputacionesOperarios
                .Where(i => i.IdOperacion == idOperacion)
                .GroupBy(i => new { i.Empleado.CodigoEmpleado, i.Empleado.Nombre, i.Empleado.Apellidos })
                .Select(g => new OperarioHorasDTO
                {
                    NombreCompleto = g.Key.Nombre + " " + g.Key.Apellidos,
                    CodigoOperario = g.Key.CodigoEmpleado,
                    TotalHoras = g.Sum(i => i.Horas)
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("material/generar-codigo/{nombreMaterial}")]
        public async Task<IActionResult> GenerarCodigoMaterial(string nombreMaterial)
        {
            string limpio = new string((nombreMaterial ?? "MAT").Trim().ToUpper().Where(char.IsLetter).ToArray());
            string prefijo = limpio.Length >= 3 ? limpio.Substring(0, 3) : limpio.PadRight(3, 'X');
            int contador = await _context.Materiales.CountAsync(m => m.CodigoMaterial.StartsWith(prefijo + "-"));

            return Ok($"{prefijo}-{(contador + 1):D3}");
        }

        [HttpPost("fichaje/terminal")]
        public async Task<IActionResult> ImputarTrabajoDesdeTerminalAsync([FromBody] FichajeTerminalReq req)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var op = await _context.OperacionesOrden.FindAsync(req.IdOperacion);
                if (op != null)
                {
                    op.PiezasFabricadas += req.PiezasHechasTurno;
                    op.PiezasRotas += req.PiezasRotasTurno;

                    if (op.IdMaquina == 0) op.IdMaquina = null;
                    if (op.IdSeccion == 0) op.IdSeccion = null;
                }

                var maq = await _context.Maquinas.FindAsync(req.IdMaquina);
                if (maq != null)
                {
                    maq.CiclosReales += req.PiezasHechasTurno;
                    maq.FechaActualizacion = DateTime.Now;
                }

                var imputacionAbierta = await _context.ImputacionesOperarios.FirstOrDefaultAsync(i => i.IdOperacion == req.IdOperacion
                    && i.IdEmpleado == req.IdEmpleado && i.FechaFin == null);

                if(imputacionAbierta != null){

                    imputacionAbierta.FechaFin = req.Fin;
                    imputacionAbierta.Horas = (decimal)Math.Round((req.Fin - imputacionAbierta.FechaInicio.Value).TotalHours, 4);
                    imputacionAbierta.PiezasFabricadas = req.PiezasHechasTurno;
                    imputacionAbierta.PiezasRotas = req.PiezasRotasTurno;
                }
                else
                {
                    _context.ImputacionesOperarios.Add(new ImputacionOperario
                    {
                        IdOperacion = req.IdOperacion,
                        IdEmpleado = req.IdEmpleado,
                        FechaRegistro = DateTime.Now,
                        FechaInicio = req.Inicio,
                        FechaFin = req.Fin,
                        Horas = Math.Round((decimal)(req.Fin - req.Inicio).TotalHours, 2),
                        PiezasFabricadas = req.PiezasHechasTurno,
                        PiezasRotas = req.PiezasRotasTurno
                    });
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return Ok(true);
            }
            catch
            {
                await tx.RollbackAsync();
                return Ok(false);
            }
        }

        [HttpPut("operacion/cerrar/{idOperacion}/{idMaquina}/{idSeccion}")]
        public async Task<IActionResult> CerrarOperacionAsync(int idOperacion, int idMaquina, int idSeccion)
        {
            try
            {
                var op = await _context.OperacionesOrden.FindAsync(idOperacion);
                if (op == null) return Ok(false);

                op.Estado = "Finalizado";
                op.IdMaquina = idMaquina == 0 ? null : idMaquina;
                op.IdSeccion = idSeccion == 0 ? null : idSeccion;
                op.FechaFin = DateTime.Now;

                await _context.SaveChangesAsync();

                if (!await _context.OperacionesOrden.AnyAsync(o => o.IdOrden == op.IdOrden && o.Estado != "Finalizado"))
                {
                    var orden = await _context.Ordenes.FindAsync(op.IdOrden);
                    if (orden != null)
                    {
                        orden.Estado = "Finalizado";
                        orden.FechaFin = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }
                }
                return Ok(true);
            }
            catch
            {
                return Ok(false);
            }
        }

        [HttpGet("operacion/verificar-cerrada/{idOperacion}")]
        public async Task<IActionResult> VerificarOperacionCerrada(int idOperacion)
        {
            var op = await _context.OperacionesOrden.FindAsync(idOperacion);
            return Ok(op != null && op.Estado == "Finalizado");
        }

        [HttpGet("maquina/{idMaquina}/materiales")]
        public async Task<IActionResult> ObtenerMaterialesPorMaquinaAsync(int idMaquina)
        {
            var ids = await _context.MaquinasMateriales
                .Where(mm => mm.IdMaquina == idMaquina)
                .Select(mm => mm.IdMaterial)
                .ToListAsync();

            if (ids.Any())
            {
                var materiales = await _context.Materiales.Where(m => ids.Contains(m.Id)).ToListAsync();
                return Ok(materiales);
            }

            return Ok(new List<Material>());
        }

        [HttpPost("material/consumo-normal")]
        public async Task<IActionResult> RegistrarConsumoMaterialAsync([FromBody] ConsumoMaterialReq req)
        {
            try
            {
                _context.ImputacionMateriales.Add(new ImputacionMaterial
                {
                    IdOperacion = req.IdOperacion,
                    IdMaterial = req.IdMaterial,
                    IdEmpleado = req.IdEmpleado,
                    Cantidad = req.Cantidad,
                    Observaciones = req.Observaciones,
                    FechaRegistro = DateTime.Now
                });

                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch
            {
                return Ok(false);
            }
        }

        [HttpGet("operacion/{idOperacion}/consumos")]
        public async Task<IActionResult> ObtenerConsumosPorOperacionAsync(int idOperacion)
        {
            var consumos = await _context.ImputacionMateriales
                .AsNoTracking()
                .Where(im => im.IdOperacion == idOperacion)
                .OrderByDescending(im => im.FechaRegistro)
                .ToListAsync();

            var materiales = await _context.Materiales.AsNoTracking().ToListAsync();

            foreach (var c in consumos)
            {
                c.Material = materiales.FirstOrDefault(m => m.Id == c.IdMaterial);
            }

            return Ok(consumos);
        }

        [HttpPut("material/restar-stock/{idMaterial}/{cantidad}")]
        public async Task<IActionResult> RestadoStockAsync(int idMaterial, decimal cantidad)
        {
            try
            {
                var m = await _context.Materiales.FindAsync(idMaterial);

                if (m == null || m.Stock < cantidad) return Ok(false);

                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch
            {
                return Ok(false);
            }
        }

        [HttpGet("material/stock-minimo/{idMaquina}/{cantidad}")]
        public async Task<IActionResult> StockMinimoAsync(int idMaquina, decimal cantidad)
        {
            var m = await _context.Materiales.FindAsync(idMaquina);
            return Ok(m != null && m.StockMinimo > (m.Stock - cantidad));
        }

        [HttpDelete("material/consumo/{idConsumo}")]
        public async Task<IActionResult> EliminarConsumoAsync(int idConsumo)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var c = await _context.ImputacionMateriales.FindAsync(idConsumo);
                if (c == null) return Ok(false);

                var m = await _context.Materiales.FindAsync(c.IdMaterial);
                if (m != null) m.Stock += c.Cantidad;

                _context.ImputacionMateriales.Remove(c);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(true);
            }
            catch
            {
                await tx.RollbackAsync();
                return Ok(false);
            }
        }

        [HttpPost("material/consumo-merma")]
        public async Task<IActionResult> RegistrarConsumoMaterialMermaAsync([FromBody] ConsumoMermaReq req)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var m = await _context.Materiales.FindAsync(req.IdMaterial);
                if (m != null && m.Stock > req.Cantidad)
                {
                    m.Stock -= req.Cantidad;
                }

                _context.ImputacionMateriales.Add(new ImputacionMaterial
                {
                    IdOperacion = req.IdOperacion,
                    IdMaterial = req.IdMaterial,
                    Cantidad = req.Cantidad,
                    IdEmpleado = req.IdEmpleado,
                    EsMerma = req.EsMerma,
                    FechaRegistro = DateTime.Now
                });

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(true);
            }
            catch
            {
                await tx.RollbackAsync();
                return Ok(false);
            }
        }

        [HttpGet("operacion/puede-iniciar/{idOperacion}/{idOrden}")]
        public async Task<IActionResult> PuedeIniciarOperacionAsync(int idOperacion, int idOrden)
        {
            var op = await _context.OperacionesOrden.FindAsync(idOperacion);
            if (op == null) return Ok(false);

            var bloqueo = await _context.OperacionesOrden.AnyAsync(o =>
                o.IdOrden == idOrden &&
                o.Preferencia > op.Preferencia &&
                o.Estado != "Finalizado");

            return Ok(!bloqueo);
        }

        [HttpPost("fichaje/iniciar-basico/{idOperacion}/{idEmpleado}")]
        public async Task<IActionResult> IniciarFichajeAsync(int idOperacion, int idEmpleado)
        {
            var op = await _context.OperacionesOrden.FindAsync(idOperacion);
            if (op != null)
            {
                op.Estado = "En curso";
                op.FechaInicio = DateTime.Now;

                var orden = await _context.Ordenes.FindAsync(op.IdOrden);
                if (orden != null && orden.Estado == "Pendiente")
                {
                    orden.Estado = "En curso";
                }

                var maq = await _context.Maquinas.FindAsync(op.IdMaquina);
                if (maq != null)
                {
                    maq.EstadoActualId = 1;
                }

                _context.ImputacionesOperarios.Add(new ImputacionOperario
                {
                    IdOperacion = idOperacion,
                    IdEmpleado = idEmpleado,
                    FechaInicio = DateTime.Now
                });

                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPost("orden/{idOrden}/hoja-ruta")]
        public async Task<IActionResult> GenerarHojaRutaEndpointAsync(int idOrden)
        {
            await GenerarHojaRutaInt(idOrden);
            return Ok();
        }

        private async Task GenerarHojaRutaInt(int idOrden)
        {
            var orden = await _context.Ordenes.FindAsync(idOrden);
            if (orden == null) return;

            var ops = await _context.TiposOperaciones.OrderByDescending(o => o.Preferencia).ToListAsync();

            foreach (var op in ops)
            {
                _context.OperacionesOrden.Add(new OperacionesOrden
                {
                    IdOrden = idOrden,
                    IdOperacionMaestra = op.Id,
                    Preferencia = op.Preferencia,
                    Estado = "Pendiente",
                    CodigoOperacion = op.Nombre,
                    CiclosObjetivo = 0,
                    PiezasRotas = 0,
                    PiezasFabricadas = 0
                });
            }

            await _context.SaveChangesAsync();
        }

        [HttpGet("tipos-operaciones")]
        public async Task<IActionResult> ObtenerOperacionesActivas()
        {
            var operaciones = await _context.TiposOperaciones.OrderByDescending(o => o.Preferencia).ToListAsync();
            return Ok(operaciones);
        }

        [HttpPut("operacion/vincular/{idOperacion}/{idMaquina}")]
        public async Task<IActionResult> VincularMaquinaAOperacionAsync(int idOperacion, int idMaquina)
        {
            try
            {
                var op = await _context.OperacionesOrden.FindAsync(idOperacion);
                if (op != null)
                {
                    op.Estado = "En curso";
                    op.IdMaquina = idMaquina == 0 ? null : idMaquina;

                    var maq = await _context.Maquinas.FindAsync(idMaquina);
                    if (maq != null)
                    {
                        maq.EstadoActualId = 1;
                    }

                    var ord = await _context.Ordenes.FindAsync(op.IdOrden);
                    if (ord != null && ord.Estado == "Pendiente")
                    {
                        ord.Estado = "En curso";
                    }

                    await _context.SaveChangesAsync();
                    return Ok(true);
                }
                return Ok(false);
            }
            catch
            {
                return Ok(false);
            }
        }

        [HttpPut("operacion/liberar/{idOperacion}/{idMaquina}")]
        public async Task<IActionResult> LiberarMaquinaYOperacionAsync(int idOperacion, int idMaquina)
        {
            try
            {
                var op = await _context.OperacionesOrden.FindAsync(idOperacion);
                if (op != null && op.Estado == "En curso")
                {
                    op.IdMaquina = null;
                }

                var maq = await _context.Maquinas.FindAsync(idMaquina);
                if (maq != null && maq.EstadoActualId == 1)
                {
                    maq.EstadoActualId = 3;
                }

                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch
            {
                return Ok(false);
            }
        }

        [HttpGet("operacion/{idOperacionDiaria}/materiales-permitidos")]
        public async Task<IActionResult> ObtenerMaterialesPermitidosAsync(int idOperacionDiaria)
        {
            try
            {
                var op = await _context.OperacionesOrden.FindAsync(idOperacionDiaria);
                if (op == null) return Ok(new List<Material>());

                var tipo = await _context.TiposOperaciones.FirstOrDefaultAsync(t => t.Nombre == op.CodigoOperacion);
                if (tipo == null) return Ok(new List<Material>());

                var materialesPermitidos = await _context.TiposOperacionesMateriales
                    .Include(tm => tm.Material)
                    .Where(tm => tm.IdTipoOperacion == tipo.Id)
                    .Select(tm => tm.Material)
                    .ToListAsync();

                return Ok(materialesPermitidos);
            }
            catch
            {
                return Ok(new List<Material>());
            }
        }

        [HttpGet("operacion/{idOperacion}/imputaciones")]
        public async Task<IActionResult> ObtenerImputacionMaterialesAsync(int idOperacion)
        {
            var imputaciones = await _context.ImputacionMateriales
                .Include(im => im.Material)
                .Include(im => im.Empleado)
                .Where(im => im.IdOperacion == idOperacion)
                .OrderByDescending(im => im.FechaRegistro)
                .ToListAsync();

            return Ok(imputaciones);
        }

        [HttpGet("orden/{idOrden}/materiales-consumidos")]
        public async Task<IActionResult> ObtenerMaterialesPorOrdenAsync(int idOrden)
        {
            var materialesConsumidos = await _context.ImputacionMateriales
                .Include(im => im.Material)
                .Include(im => im.Empleado)
                .Where(im => im.OperacionesOrden.IdOrden == idOrden)
                .OrderByDescending(im => im.FechaRegistro)
                .ToListAsync();

            return Ok(materialesConsumidos);
        }

        [HttpGet("orden/{idOrden}/operaciones-detalle")]
        public async Task<IActionResult> ObtenerOperacionesOrdenAsync(int idOrden)
        {
            var operaciones = await _context.OperacionesOrden
                .Include(o => o.DetalleOperacion)
                .Include(o => o.Imputaciones)
                .Where(o => o.IdOrden == idOrden)
                .OrderByDescending(o => o.Preferencia)
                .ToListAsync();

            return Ok(operaciones);
        }

        [HttpPost("fichaje/iniciar-reanudar/{idOperacion}/{idEmpleado}")]
        public async Task<IActionResult> IniciarOReanudarFichajeAsync(int idOperacion, int idEmpleado)
        {
            var op = await _context.OperacionesOrden
                .Include(o => o.Orden)
                .FirstOrDefaultAsync(o => o.Id == idOperacion);

            if (op == null) return Ok();

            if (op.FechaInicio == null)
            {
                op.FechaInicio = DateTime.Now;
            }
            op.Estado = "En curso";

            if (op.Orden != null && op.Orden.Estado == "Pendiente")
            {
                op.Orden.Estado = "En curso";
                if (op.Orden.FechaInicio == null)
                {
                    op.Orden.FechaInicio = DateTime.Now;
                }
            }

            if (op.IdMaquina != null)
            {
                var maq = await _context.Maquinas.FindAsync(op.IdMaquina);
                if (maq != null) maq.EstadoActualId = 1;
            }

            _context.ImputacionesOperarios.Add(new ImputacionOperario
            {
                IdOperacion = idOperacion,
                IdEmpleado = idEmpleado,
                FechaInicio = DateTime.Now,
                FechaFin = null,
                Horas = 0
            });

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("fichaje/pausar/{idOperacion}/{idEmpleado}")]
        public async Task<IActionResult> PausarFichajeAsync(int idOperacion, int idEmpleado)
        {
            var imp = await _context.ImputacionesOperarios.FirstOrDefaultAsync(i =>
                i.IdOperacion == idOperacion &&
                i.IdEmpleado == idEmpleado &&
                i.FechaFin == null);

            if (imp != null)
            {
                imp.FechaFin = DateTime.Now;
                imp.Horas = (decimal)Math.Round((imp.FechaFin.Value - imp.FechaInicio.Value).TotalHours, 4);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPut("fichaje/finalizar/{idOperacion}/{idEmpleado}")]
        public async Task<IActionResult> FinalizarOperacionAsync(int idOperacion, int idEmpleado)
        {
            var imp = await _context.ImputacionesOperarios.FirstOrDefaultAsync(i =>
                i.IdOperacion == idOperacion &&
                i.IdEmpleado == idEmpleado &&
                i.FechaFin == null);

            if (imp != null)
            {
                imp.FechaFin = DateTime.Now;
                imp.Horas = (decimal)Math.Round((imp.FechaFin.Value - imp.FechaInicio.Value).TotalHours, 4);
            }

            var op = await _context.OperacionesOrden.FindAsync(idOperacion);
            if (op != null)
            {
                op.Estado = "Finalizado";
                op.FechaFin = DateTime.Now;

                if (op.IdMaquina != null)
                {
                    var maq = await _context.Maquinas.FindAsync(op.IdMaquina);
                    if (maq != null) maq.EstadoActualId = 3;
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("orden/progreso")]
        public async Task<IActionResult> ObtenerOrdenesConProgresoAsync()
        {
            var ordenes = await _context.Ordenes
                .Include(o => o.Operaciones)
                    .ThenInclude(op => op.DetalleOperacion)
                .Include(o => o.Operaciones)
                    .ThenInclude(op => op.Imputaciones)
                .ToListAsync();

            foreach (var o in ordenes)
            {
                double teorico = o.Operaciones
                    .Where(op => op.DetalleOperacion != null)
                    .Sum(op => op.DetalleOperacion.TiempoTeorico);

                double reales = 0;

                foreach (var op in o.Operaciones)
                {
                    if (op.Imputaciones != null)
                    {
                        reales += op.Imputaciones
                            .Where(i => i.FechaFin != null && i.FechaInicio != null)
                            .Sum(i => (i.FechaFin.Value - i.FechaInicio.Value).TotalHours);

                        foreach (var act in op.Imputaciones.Where(i => i.FechaFin == null && i.FechaInicio != null))
                        {
                            reales += (DateTime.Now - act.FechaInicio.Value).TotalHours;
                        }
                    }
                }

                o.PorcentajeTiempoTotal = teorico > 0 ? (int)Math.Min((reales / (teorico / 60)) * 100, 100) : 0;
            }

            return Ok(ordenes);
        }

        [HttpDelete("fichaje/abierto/{idOperacion}/{idEmpleado}")]
        public async Task<IActionResult> EliminarFichajeAbiertoAsync(int idOperacion, int idEmpleado)
        {
            var fichajeBorrar = await _context.ImputacionesOperarios.FirstOrDefaultAsync(i => i.IdOperacion == idOperacion && i.IdEmpleado == idEmpleado && i.FechaFin == null);

            if(fichajeBorrar != null)
            {
                _context.ImputacionesOperarios.Remove(fichajeBorrar);
                await _context.SaveChangesAsync();
                return Ok(true);
            }
            return Ok(false);
        }

        public class CierreOperacionRequest
        {
            public int Id { get; set; }
            public int IdMaquina { get; set; }
            public DateTime FechaFin { get; set; }
            public int PiezasFabricadas { get; set; }
            public int PiezasRotas { get; set; }
        }

        public class FichajeTerminalReq
        {
            public int IdOperacion { get; set; }
            public int IdMaquina { get; set; }
            public int IdEmpleado { get; set; }
            public DateTime Inicio { get; set; }
            public DateTime Fin { get; set; }
            public int PiezasHechasTurno { get; set; }
            public int PiezasRotasTurno { get; set; }
        }

        public class ConsumoMaterialReq
        {
            public int IdOperacion { get; set; }
            public int IdMaterial { get; set; }
            public decimal Cantidad { get; set; }
            public int IdEmpleado { get; set; }
            public string? Observaciones { get; set; }
        }

        public class ConsumoMermaReq
        {
            public int IdOperacion { get; set; }
            public int IdMaterial { get; set; }
            public decimal Cantidad { get; set; }
            public int IdEmpleado { get; set; }
            public bool EsMerma { get; set; }
        }
    }
}