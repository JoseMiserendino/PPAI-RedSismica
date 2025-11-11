using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAI_V2.entidades
{
    public class OrigenDeGeneracion
    {
        String descripcion;
        String nombre;

        public OrigenDeGeneracion(String nombre, String descripcion)
        {
            this.nombre = nombre;
            this.descripcion = descripcion;
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
