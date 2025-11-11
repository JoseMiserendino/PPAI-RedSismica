using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAI_V2.entidades
{
    public class Empleado
    {
        public int Id { get; set; }
        public String apellido;
        public String nombre;
        public String mail;
        public String telefono;

        public Empleado(int id, String apellido, String nombre, String mail, String telefono)
        {
            this.Id = id;
            this.apellido = apellido;
            this.nombre = nombre;
            this.mail = mail;
            this.telefono = telefono;
        }

        public override string ToString()
        {
            return $"Apellido: {apellido} - Nombre: {nombre} - Mail: {mail} - Telefono: {telefono}";
        }
    }
}
