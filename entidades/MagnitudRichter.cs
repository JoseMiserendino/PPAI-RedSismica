using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAI_V2.entidades
{
    internal class MagnitudRichter
    {
        String descripcionMagnitud;
        double numero;

        public MagnitudRichter(String descripcionMagnitud, double numero)
        {
            this.descripcionMagnitud = descripcionMagnitud;
            this.numero = numero;
        }

        public double Numero
        {
            get { return numero; }
            set { numero = value; }
        }
    }
}
