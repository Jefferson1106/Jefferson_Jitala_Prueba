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
    public class ClientesController : ControllerBase
    {
        private readonly BaseJitalaContext _context;

        public ClientesController(BaseJitalaContext context)
        {
            _context = context;
        }

        // GET: api/Clientes
        [HttpGet]
        public async Task<ActionResult<List<Cliente>>> GetClientes()
        {
            return await _context.Clientes.ToListAsync();
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        // PUT: api/Clientes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<MesajeRespuesta> PutCliente(int id, Cliente cliente)
        {
            MesajeRespuesta respuesta = new MesajeRespuesta();
            if (id != cliente.ClIdCliente)
            {
                respuesta.Message = "Cliente ya existe con la identificacion ingresada";
                return respuesta;
            }

            _context.Entry(cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                respuesta.Message = "Cliente modificado Correctamente";
                respuesta.Resultado = true;
                return respuesta;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (ClienteExists(cliente.Identificacion))
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

        // POST: api/Clientes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<MesajeRespuesta> PostCliente(Cliente cliente)
        {
            MesajeRespuesta respuesta = new MesajeRespuesta();
            if (ClienteExists(cliente.Identificacion))
            {
                respuesta.Message = "Cliente no se puedo modificado Correctamente";
                respuesta.IsSuccess = false;
                return respuesta;
            }
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            respuesta.Message = "Cliente se creo Correctamente";
            respuesta.IsSuccess = true;
            respuesta.Resultado = cliente;

            return respuesta;
        }

        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public async Task<MesajeRespuesta> DeleteCliente(int id)
        {
            MesajeRespuesta respuesta = new MesajeRespuesta();
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                respuesta.Message = "Cliente no se puedo eliminar";
                respuesta.Resultado = false;
                return respuesta;
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            respuesta.Message = "Cliente eliminado";
            respuesta.Resultado = false;
            return respuesta;
        }

        private bool ClienteExists(string id)
        {
            return _context.Clientes.Any(e => e.Identificacion == id);
        }
    }
}
