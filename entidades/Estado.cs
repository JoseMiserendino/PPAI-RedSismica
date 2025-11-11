using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAI_V2.entidades
{
    public abstract class Estado
    {
        private String ambito;
        private String nombreEstado;

        public Estado(string ambito, string nombreEstado)
        {
            this.ambito = ambito;
            this.nombreEstado = nombreEstado;
        }

        public string Ambito
        {
            get { return ambito; }
            set { ambito = value; }
        }

        public string NombreEstado
        {
            get { return nombreEstado; }
            set { nombreEstado = value; }
        }

        public bool EsAutoDetectado()
        {
            return nombreEstado == "AutoDetectado";
        }

        public bool EsRechazado()
        {
            return nombreEstado == "Rechazado";
        }

        public bool EsAmbitoEventoSismico()
        {
            return ambito == "EventoSismico";
        }

        public bool EsBloqueadoEnRevision()
        {
            return nombreEstado == "BloqueadoEnRevision";
        }

        public virtual void AdquirirDatos()
        {
            throw new InvalidOperationException(
                $"No se puede ejecutar AdquirirDatos en el estado {nombreEstado}");
        }

        public virtual void Cerrar()
        {
            throw new InvalidOperationException(
                $"No se puede ejecutar Cerrar en el estado {nombreEstado}");
        }

        public virtual void Confirmar()
        {
            throw new InvalidOperationException(
                $"No se puede ejecutar Confirmar en el estado {nombreEstado}");
        }

        public virtual void Rechazar(DateTime fh, Empleado usuarioLogueado,
            CambioEstado[] cambiosEstado, EventoSismico evento)
        {
            throw new InvalidOperationException(
                $"No se puede ejecutar Rechazar en el estado {nombreEstado}");
        }

        public virtual void Derivar()
        {
            throw new InvalidOperationException(
                $"No se puede ejecutar Derivar en el estado {nombreEstado}");
        }

        public virtual void ControlarTiempo()
        {
            throw new InvalidOperationException(
                $"No se puede ejecutar ControlarTiempo en el estado {nombreEstado}");
        }

        public virtual void Revisar(DateTime fh, Empleado usuarioLogueado,
            CambioEstado[] cambiosEstado, EventoSismico evento)
        {
            throw new InvalidOperationException(
                $"No se puede ejecutar Revisar en el estado {nombreEstado}");
        }

        public virtual void Anular()
        {
            throw new InvalidOperationException(
                $"No se puede ejecutar Anular en el estado {nombreEstado}");
        }

        public override string ToString()
        {
            return $"Ambito: {ambito} - Nombre: {nombreEstado}";
        }
    }
}
