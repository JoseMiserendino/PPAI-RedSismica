using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAI_V2.entidades
{
    public class SerieTemporal
    {
        // Atributos
        private double condicionAlarma;
        private DateTime fechaHoraInicioRegistroMuestras;
        private DateTime fechaHoraFinRegistro;
        private double frecuenciaMuestreo;

        // Atributos referenciales
        List<MuestraSismica> muestrasSismicas;

        public SerieTemporal(double condicionAlarma, DateTime fechaHoraInicioRegistroMuestras, DateTime fechaHoraFinRegistro, double frecuenciaMuestreo, List<MuestraSismica> muestrasSismicas)
        {
            this.condicionAlarma = condicionAlarma;
            this.fechaHoraInicioRegistroMuestras = fechaHoraInicioRegistroMuestras;
            this.fechaHoraFinRegistro = fechaHoraFinRegistro;
            this.frecuenciaMuestreo = frecuenciaMuestreo;
            this.muestrasSismicas = muestrasSismicas;
        }


        public double CondicionAlarma => condicionAlarma;
        public DateTime FechaHoraInicioRegistroMuestras => fechaHoraInicioRegistroMuestras;
        public DateTime FechaHoraFinRegistro => fechaHoraFinRegistro;
        public double FrecuenciaMuestreo => frecuenciaMuestreo;
        public List<MuestraSismica> MuestrasSismicas => muestrasSismicas;

        public string GetDatos()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Serie Temporal desde {fechaHoraInicioRegistroMuestras:yyyy-MM-dd HH:mm:ss}");
            foreach (var muestra in muestrasSismicas)
            {
                sb.AppendLine($"  {muestra.GetDatos()}");
            }
            return sb.ToString();
        }
    }
}
