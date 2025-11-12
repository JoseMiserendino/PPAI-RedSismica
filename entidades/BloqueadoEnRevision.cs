using PPAI_V2.daos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAI_V2.entidades
{
    public class BloqueadoEnRevision : Estado
    {
        public BloqueadoEnRevision() : base("EventoSismico", "BloqueadoEnRevision")
        {
        }

        public override void Confirmar(DateTime fh, Empleado usuarioLogueado, CambioEstado[] cambiosEstado, EventoSismico evento)
        {
            finalizarCambioEstadoActual(fh, usuarioLogueado, cambiosEstado);
            CambioEstadoDAO cambioDAO = new CambioEstadoDAO();
            cambioDAO.ActualizarFechaFinUltimoCambio(evento.Id, fh);
            ConfirmadoPorPersonal confirmado = crearEstadoConfirmado();
            CambioEstado nuevoCambioEstado = crearCambioEstado(fh, usuarioLogueado, confirmado);
            evento.cambiarEstado(confirmado, nuevoCambioEstado);
        }

        public override void Derivar(DateTime fh, Empleado usuarioLogueado, CambioEstado[] cambiosEstado, EventoSismico evento)
        {
            finalizarCambioEstadoActual(fh, usuarioLogueado, cambiosEstado);
            CambioEstadoDAO cambioDAO = new CambioEstadoDAO();
            cambioDAO.ActualizarFechaFinUltimoCambio(evento.Id, fh);
            Derivado derivado = crearEstadoDerivado();
            CambioEstado nuevoCambioEstado = crearCambioEstado(fh, usuarioLogueado, derivado);
            evento.cambiarEstado(derivado, nuevoCambioEstado);
        }

        public Derivado crearEstadoDerivado()
        {
            return new Derivado();
        }

        public ConfirmadoPorPersonal crearEstadoConfirmado()
        {
            return new ConfirmadoPorPersonal();
        }

        public override void Rechazar(DateTime fh, Empleado usuarioLogueado, CambioEstado[] cambiosEstado, EventoSismico evento)
        {
            finalizarCambioEstadoActual(fh, usuarioLogueado, cambiosEstado);
            CambioEstadoDAO cambioDAO = new CambioEstadoDAO();
            cambioDAO.ActualizarFechaFinUltimoCambio(evento.Id, fh);
            Rechazado rechazado = crearEstadoRechazado();
            CambioEstado nuevoCambioEstado = crearCambioEstado(fh, usuarioLogueado, rechazado);
            evento.cambiarEstado(rechazado, nuevoCambioEstado);
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

        public Rechazado crearEstadoRechazado()
        {
            return new Rechazado();
        }

        public CambioEstado crearCambioEstado(DateTime fh, Empleado empleado, Estado estado)
        {
            return new CambioEstado(fh, estado, empleado);
        }
    }
}
