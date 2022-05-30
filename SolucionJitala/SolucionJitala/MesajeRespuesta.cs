using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolucionJitala
{
    public class MesajeRespuesta
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Resultado { get; set; }
    }
}
