using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAI_V2.entidades
{
    internal class MuestraSismica
    {
        // Atributos
        private DateTime fechaHoraMuestra;

        // Atributos referenciales
        List<DetalleMuestraSismica> detalleMuestraSismica;

        public MuestraSismica(DateTime fechaHoraMuestra, List<DetalleMuestraSismica> detalleMuestraSismica)
        {
            this.fechaHoraMuestra = fechaHoraMuestra;
            this.detalleMuestraSismica = detalleMuestraSismica;
        }

        public DateTime FechaHoraMuestra
        {
            get { return fechaHoraMuestra; }
            set { fechaHoraMuestra = value; }
        }

        public string GetDatos()
        {
            var datos = detalleMuestraSismica.Select(d => d.GetDatos())
                .ToDictionary(d => d.Nombre, d => d.Valor);

            double? velocidad = null;
            double? frecuencia = null;
            double? longitud = null;

            if (datos.TryGetValue("Velocidad de onda", out double vel))
                velocidad = vel;
            if (datos.TryGetValue("Frecuencia de onda", out double freq))
                frecuencia = freq;
            if (datos.TryGetValue("Longitud de onda", out double lon))
                longitud = lon;

            return $"Muestra a las {fechaHoraMuestra:yyyy-MM-dd HH:mm:ss}: " +
                   $"Velocidad: {velocidad:F2} km/s, Frecuencia: {frecuencia:F2} Hz, Longitud: {longitud:F2} km/ciclo";
        }
    }
}
