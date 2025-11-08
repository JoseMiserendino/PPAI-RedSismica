using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAI_V2.entidades
{
    internal class Estado
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

        public override string ToString()
        {
            return $"Ambito: {ambito} - Nombre: {nombreEstado}";
        }
    }
}
