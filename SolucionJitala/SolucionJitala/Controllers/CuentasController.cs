using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolucionJitala.Data;
using SolucionJitala.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolucionJitala.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly BaseJitalaContext _context;

        public CuentasController(BaseJitalaContext context)
        {
            _context = context;
        }

        // GET: api/Cuentas
        [HttpGet]
        public async Task<ActionResult<List<Cuenta>>> GetCuentas()
        {
            return await _context.Cuentas.ToListAsync();
        }

        // GET: api/Cuentas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cuenta>> GetCuenta(string id)
        {
            var cuenta = await _context.Cuentas.FindAsync(id);

            if (cuenta == null)
            {
                return NotFound();
            }

            return cuenta;
        }

        // PUT: api/Cuentas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<MesajeRespuesta> PutCuenta(string id, Cuenta cuenta)
        {
            MesajeRespuesta respuesta = new MesajeRespuesta();
            if (id != cuenta.CuNumeroCuenta)
            {
                respuesta.Message = "la cuenta no existe";
                return respuesta;
            }

            _context.Entry(cuenta).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                respuesta.Message = "Cuenta modificado Correctamente";
                respuesta.Resultado = true;
                return respuesta;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CuentaExists(id))
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

        // POST: api/Cuentas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MesajeRespuesta>> PostCuenta(Cuenta cuenta)
        {
            MesajeRespuesta respuesta = new MesajeRespuesta();
            if (CuentaExists(cuenta.CuNumeroCuenta))
            {
                respuesta.Message = "cuenta no se puedo modificado Correctamente";
                respuesta.IsSuccess = false;
                return respuesta;
            }
            
            try
            {
                await _context.SaveChangesAsync();
                respuesta.Message = "Cuenta se creo Correctamente";
                respuesta.IsSuccess = true;
                respuesta.Resultado = cuenta;
                return respuesta;
            }
            catch (DbUpdateException ex)
            {
                if (CuentaExists(cuenta.CuNumeroCuenta))
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

        // DELETE: api/Cuentas/5
        [HttpDelete("{id}")]
        public async Task<MesajeRespuesta> DeleteCuenta(string id)
        {
            MesajeRespuesta respuesta = new MesajeRespuesta();
            var cuenta = await _context.Cuentas.FindAsync(id);
            if (cuenta == null)
            {
                respuesta.Message = "No se puedo eliminar la cuenta";
                return respuesta;
            }

            _context.Cuentas.Remove(cuenta);
            await _context.SaveChangesAsync();
            respuesta.Message = "Cuenta eliminado";
            respuesta.Resultado = false;
            return respuesta;
        }

        private bool CuentaExists(string id)
        {
            return _context.Cuentas.Any(e => e.CuNumeroCuenta == id);
        }
    }
}
