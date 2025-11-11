using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAI_V2.entidades
{
    public class ClasificacionSismo
    {
        private double kmProfundidadDesde;
        private double kmProfundidadHasta;
        String nombre;

        public ClasificacionSismo(String nombre, double kmProfundidadDesde, double kmProfundidadHasta)
        {
            this.nombre = nombre;
            this.kmProfundidadDesde = kmProfundidadDesde;
            this.kmProfundidadHasta = kmProfundidadHasta;
        }

        public bool EsTuNombre(String nombre)
        {
            return this.nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase);
        }

        public String Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
    }
}
