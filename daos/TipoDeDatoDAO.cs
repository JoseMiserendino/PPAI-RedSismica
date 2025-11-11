using PPAI_V2.entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAI_V2.daos
{
    internal class TipoDeDatoDAO
    {
        private Dictionary<string, TipoDeDato> cache = new Dictionary<string, TipoDeDato>();

        public TipoDeDato ObtenerOCrear(string denominacion, string unidad, double umbral)
        {
            string clave = $"{denominacion}|{unidad}";
            if (!cache.TryGetValue(clave, out TipoDeDato tipo))
            {
                tipo = new TipoDeDato(denominacion, unidad, umbral);
                cache[clave] = tipo;
            }
            return tipo;
        }
    }
}
