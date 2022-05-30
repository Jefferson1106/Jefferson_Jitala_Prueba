using SolucionJitala.Controllers;
using SolucionJitala.Model;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PruebasUnitarias
{
    public class UnitTest1: Base
    {
        [Fact]
        public async Task Test1()
        {
            //prepacion
            string nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);

            contexto.Clientes.Add(new Cliente()
            {
                Identificacion = "0604434571",
                ClContrasenia = "1234",
                ClEstado = true,
                Nombre = "Juan Perez",
                Genero = "Masculino",
                Edad = 27,
                Direccion = "quito",
                Telefono = "0945736789"
            });
            contexto.Clientes.Add(new Cliente()
            {
                Identificacion = "1704434231",
                ClContrasenia = "4321",
                ClEstado = true,
                Nombre = "Romel Garzon",
                Genero = "Masculino",
                Edad = 27,
                Direccion = "Guayaquil",
                Telefono = "0914536789"
            });
            _ = await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBD);

            //Prueba
            var controlador = new ClientesController(contexto2);
            var respuesta = await controlador.GetClientes();
            //verificacion
            var cliente = respuesta.Value;
            Assert.Equal(2, cliente.Count);

        }
        [Fact]
        public async Task Test2()
        {//prepacion
            string nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);

            contexto.Movimientos.Add(new Movimiento()
            {
                MoNumeroCuenta = "2324442",
                MoFecha = DateTime.Now,
                MoTipoMovimiento = "Deposito",
                MoSaldoInicial = 1000,
                MoMovimiento = 100,
                MoSaldoDisponible = 1100

            });
            contexto.Movimientos.Add(new Movimiento()
            {
                MoNumeroCuenta = "232222",
                MoFecha = DateTime.Now,
                MoTipoMovimiento = "Retiro",
                MoSaldoInicial = 1000,
                MoMovimiento = 200,
                MoSaldoDisponible = 800

            });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBD);

            //Prueba
            var controlador = new MovimientosController(contexto2);
            var respuesta = await controlador.GetMovimientos();
            //verificacion
            var cliente = respuesta.Value;
            Assert.Equal(2, cliente.Count);

        }
    }
}
