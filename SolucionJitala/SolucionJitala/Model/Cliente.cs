using System;
using System.Collections.Generic;

#nullable disable

namespace SolucionJitala.Model
{
    public  class Cliente:Persona
    {
        public Cliente()
        {
            Cuenta = new HashSet<Cuenta>();
        }

        public int ClIdCliente { get; set; }
        public string ClContrasenia { get; set; }
        public bool ClEstado { get; set; }
        public virtual ICollection<Cuenta> Cuenta { get; set; }
    }
}
