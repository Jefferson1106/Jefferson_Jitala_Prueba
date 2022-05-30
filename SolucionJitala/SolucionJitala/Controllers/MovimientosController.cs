using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolucionJitala.Data;
using SolucionJitala.Model;

namespace SolucionJitala.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientosController : ControllerBase
    {
        private readonly BaseJitalaContext _context;

        public MovimientosController(BaseJitalaContext context)
        {
            _context = context;
        }

        // GET: api/Movimientos
        [HttpGet]
        public async Task<ActionResult<List<Movimiento>>> GetMovimientos()
        {
            return await _context.Movimientos.ToListAsync();
        }

        // GET: api/Movimientos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movimiento>> GetMovimiento(int id)
        {
            var movimiento = await _context.Movimientos.FindAsync(id);

            if (movimiento == null)
            {
                return NotFound();
            }

            return movimiento;
        }

        // PUT: api/Movimientos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<MesajeRespuesta> PutMovimiento(int id, Movimiento movimiento)
        {
            MesajeRespuesta respuesta = new MesajeRespuesta();
            if (id != movimiento.MoIdMovimiento)
            {
                respuesta.Message = "No se puedo eliminar la cuenta";
                return respuesta;
            }

            _context.Entry(movimiento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                respuesta.Message = "Movimiento eliminado";
                respuesta.Resultado = false;
                return respuesta;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!MovimientoExists(id))
                {
                    respuesta.Message = ex.Message;
                    return respuesta;
                }
                else
                {
                    respuesta.Message = ex.Message;
                    return respuesta;
                }
            }
        }

        // POST: api/Movimientos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MesajeRespuesta>> PostMovimiento(Movimiento movimiento)
        {
            MesajeRespuesta respuesta = new MesajeRespuesta();
            movimiento.MoFecha = DateTime.Now;
            #region Validacion campos vacios
            if (movimiento.MoMovimiento <= 0)
            {
                respuesta.IsSuccess = true;
                respuesta.Message = "No se permite valores 0";
                return respuesta;
            }
            #endregion

            Movimiento ultimoMovimiento = await _context.Movimientos.Where(x => x.MoNumeroCuenta == movimiento.MoNumeroCuenta).OrderByDescending(x => x.MoFecha).FirstOrDefaultAsync();
            if(ultimoMovimiento != null)
            {
                #region Validacion de Cupos
                if (string.Equals(movimiento.MoTipoMovimiento, "Debito"))
                {
                    if (!ValidarCupos(movimiento.MoNumeroCuenta, movimiento.MoMovimiento).IsSuccess)
                    {
                        return respuesta;
                    }
                }
                #endregion
                #region Validacion de Saldos y Registro de movimientos
                if (ultimoMovimiento.MoSaldoDisponible < Math.Abs(movimiento.MoMovimiento))
                {
                    respuesta.IsSuccess = true;
                    respuesta.Message = "Saldo no disponible";
                }
                else
                {
                    movimiento.MoSaldoInicial = ultimoMovimiento.MoSaldoDisponible;
                    if (string.Equals(movimiento.MoTipoMovimiento, "Debito"))
                    {
                        movimiento.MoSaldoDisponible = ultimoMovimiento.MoSaldoDisponible - movimiento.MoMovimiento;
                        movimiento.MoMovimiento = Math.Abs(movimiento.MoMovimiento) * (-1);
                    }
                    else
                        movimiento.MoSaldoDisponible = ultimoMovimiento.MoSaldoDisponible + movimiento.MoMovimiento;
                    respuesta.IsSuccess = true;
                    _context.Movimientos.Add(movimiento);
                    await _context.SaveChangesAsync();
                }
                #endregion
            }
            else
            {
                movimiento.MoSaldoDisponible = ultimoMovimiento.MoSaldoDisponible + movimiento.MoMovimiento;
                respuesta.IsSuccess = true;
                _context.Movimientos.Add(movimiento);
                await _context.SaveChangesAsync();
            }

            respuesta.Resultado =  movimiento;
            return respuesta;
        }

        // DELETE: api/Movimientos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovimiento(int id)
        {
            var movimiento = await _context.Movimientos.FindAsync(id);
            if (movimiento == null)
            {
                return NotFound();
            }

            _context.Movimientos.Remove(movimiento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MovimientoExists(int id)
        {
            return _context.Movimientos.Any(e => e.MoIdMovimiento == id);
        }

        [HttpPost("{strNumeroCuenta}&{decMontoTransaccion}")]
        public MesajeRespuesta ValidarCupos(string strNumeroCuenta, decimal decMontoTransaccion)
        {
            MesajeRespuesta respuesta = new MesajeRespuesta();
            DateTime diaActual = DateTime.Now;

            string dia = diaActual.ToString("dd-MM-yyyy");
            DateTime InicioDeDia = DateTime.ParseExact(dia, "dd-MM-yyyy", null);
            DateTime FinalDeDia = DateTime.ParseExact(dia + " 23:59:59", "dd-MM-yyyy HH:mm:ss", null);

            decimal valoresRetiro = _context.Movimientos
                .Where(x => x.MoNumeroCuenta == strNumeroCuenta
                && x.MoFecha >= InicioDeDia
                && x.MoFecha <= FinalDeDia
                && x.MoTipoMovimiento == "Debito")
                .Sum(a => a.MoMovimiento);

            decimal totalsupuesto = Math.Abs(valoresRetiro) + Math.Abs(decMontoTransaccion);

            respuesta.Resultado = Math.Abs(totalsupuesto);
            if (totalsupuesto > 1000 )
            {
                respuesta.Message = "Cupo diario excedido";
                respuesta.IsSuccess = false;
                respuesta.Resultado = Math.Abs(valoresRetiro);
            }
            else
                respuesta.IsSuccess = true;
            return respuesta;
        }
        [HttpGet("{strIdentificacion}&{FechaInicio}&{FechaFin}")]
        public async Task<MesajeRespuesta> PostPMovimientosfechas(string strIdentificacion, string FechaInicio, string FechaFin)
        {
            List<Movimiento> lstPMovimiento = new List<Movimiento>();
            MesajeRespuesta respuesta = new MesajeRespuesta();
            try
            {
                respuesta.Resultado = await _context.Movimientos.Select(x => new MovimientosCliente
                {
                    Fecha = x.MoFecha,
                    Nombre = x.MoNumeroCuentaNavigation.CuIdClienteNavigation.Nombre,
                    NumeroCuenta = x.MoNumeroCuentaNavigation.CuNumeroCuenta,
                    Tipo = x.MoNumeroCuentaNavigation.CuTipo,
                    SaldoInicial = x.MoSaldoInicial,
                    Estado = x.MoNumeroCuentaNavigation.CuIdClienteNavigation.ClEstado,
                    Movimiento = x.MoMovimiento,
                    SaldoDisponible = x.MoSaldoDisponible,
                    Identificacion = x.MoNumeroCuentaNavigation.CuIdClienteNavigation.Identificacion,

                }).Where(s => s.Identificacion == strIdentificacion && s.Fecha >= Convert.ToDateTime(FechaInicio) && s.Fecha <= Convert.ToDateTime(FechaFin)).ToListAsync();
                respuesta.IsSuccess = true;
            }
            catch (Exception e)
            {
                respuesta.Message =  e.StackTrace;
            }
            return respuesta;
        }
    }
}
