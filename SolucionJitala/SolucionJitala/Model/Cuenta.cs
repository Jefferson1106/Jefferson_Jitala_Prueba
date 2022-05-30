using System;
using System.Collections.Generic;

#nullable disable

namespace SolucionJitala.Model
{
    public  class Cuenta
    {
        public Cuenta()
        {
            Movimientos = new HashSet<Movimiento>();
        }

        public string CuNumeroCuenta { get; set; }
        public int CuIdCliente { get; set; }
        public string CuTipo { get; set; }
        public bool CuEstado { get; set; }

        public virtual Cliente CuIdClienteNavigation { get; set; }
        public virtual ICollection<Movimiento> Movimientos { get; set; }
    }
}
