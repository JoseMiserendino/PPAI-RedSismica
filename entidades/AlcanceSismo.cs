using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAI_V2.entidades
{
    public class AlcanceSismo
    {
        private String descripcion;
        private String nombre;

        public AlcanceSismo(String nombre, String descripcion)
        {
            this.nombre = nombre;
            this.descripcion = descripcion;
        }

        public String Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }

        public bool EsTuNombre(String nombre)
        {
            return this.nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase);
        }
    }
}
