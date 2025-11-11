using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAI_V2.entidades
{
    public class EstacionSismologica
    {
        private string codigoEstacion;
        private string documentoCertificacionAdq;
        private DateTime fechaSolicitudCertificacion;
        private double latitud;
        private double longitud;
        private string nombre;
        private double nroCertificacionAdquisicion;

        public EstacionSismologica(
            string codigoEstacion,
            string documentoCertificacionAdq,
            DateTime fechaSolicitudCertificacion,
            double latitud,
            double longitud,
            string nombre,
            double nroCertificacionAdquisicion)
        {
            this.codigoEstacion = codigoEstacion;
            this.documentoCertificacionAdq = documentoCertificacionAdq;
            this.fechaSolicitudCertificacion = fechaSolicitudCertificacion;
            this.latitud = latitud;
            this.longitud = longitud;
            this.nombre = nombre;
            this.nroCertificacionAdquisicion = nroCertificacionAdquisicion;
        }

        public string Nombre => nombre;

        public override string ToString()
        {
            return $"Estación: {nombre}, Ubicación: ({latitud:F6}, {longitud:F6})";
        }
    }
}
