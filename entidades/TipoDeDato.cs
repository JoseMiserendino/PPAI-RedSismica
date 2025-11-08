using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAI_V2.entidades
{
    internal class TipoDeDato
    {
        private String denominacion;
        private String nombreUnidadMedida;
        private double valorUmbral;

        public TipoDeDato(String denominacion, String nombreUnidadMedida, double valorUmbral)
        {
            this.denominacion = denominacion;
            this.nombreUnidadMedida = nombreUnidadMedida;
            this.valorUmbral = valorUmbral;
        }

        public String Denominacion
        {
            get { return denominacion; }
            set { denominacion = value; }
        }
    }
}
