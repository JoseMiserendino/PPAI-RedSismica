using PPAI_V2.daos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAI_V2.entidades
{
    public class AutoDetectado : Estado
    {
        public AutoDetectado() : base("EventoSismico", "AutoDetectado")
        {
        }

        public override void Revisar(DateTime fh, Empleado usuarioLogueado, CambioEstado[] cambiosEstado, EventoSismico evento)
        {
            Console.WriteLine("======= Revisando evento auto-detectado... =========");
            finalizarCambioEstadoActual(fh, usuarioLogueado, cambiosEstado);
            CambioEstadoDAO cambioDAO = new CambioEstadoDAO();
            cambioDAO.ActualizarFechaFinUltimoCambio(evento.Id, fh);
            BloqueadoEnRevision bloqueadoEnRevision = crearEstadoBloqueadoEnRevision();
            CambioEstado nuevoCambioEstado = crearCambioEstado(fh, usuarioLogueado, bloqueadoEnRevision);
            evento.cambiarEstado(bloqueadoEnRevision, nuevoCambioEstado);
        }
        
        public void finalizarCambioEstadoActual(DateTime fh, Empleado usuarioLogueado, CambioEstado[] cambiosEstado)
        {
            foreach (CambioEstado c in cambiosEstado)
            {
                if (c.EsEstadoActual())
                {
                    c.FechaHoraFin = fh;
                }
            }
        }

        public BloqueadoEnRevision crearEstadoBloqueadoEnRevision()
        {
            return new BloqueadoEnRevision();
        }

        public CambioEstado crearCambioEstado(DateTime fh, Empleado empleado, Estado estado)
        {
            return new CambioEstado(fh, estado, empleado);
        }
    }
}
