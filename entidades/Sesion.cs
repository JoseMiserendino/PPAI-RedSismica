using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAI_V2.entidades
{
    internal class Sesion
    {
        private static Sesion instancia; // Singleton para simular una única sesión
        private Empleado usuarioLogueado;

        private Sesion(Empleado usuario)
        {
            usuarioLogueado = usuario;
        }

        public static Sesion IniciarSesion(Empleado usuario)
        {
            if (instancia == null)
            {
                instancia = new Sesion(usuario);
            }
            return instancia;
        }

        public Empleado ObtenerUsuarioLogueado()
        {
            return usuarioLogueado;
        }

        // Método para cerrar sesión (opcional)
        public static void CerrarSesion()
        {
            instancia = null;
        }
    }
}
