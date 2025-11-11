using PPAI_V2.daos;
using PPAI_V2.gestor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAI_V2.entidades
{
    public class EventoSismico
    {

        // Atributos
        private int id;
        private DateTime fechaHoraFin;
        private DateTime fechaHoraOcurrencia;
        private double latitudEpicentro;
        private double longitudEpicentro;
        private double latitudHipocentro;
        private double longitudHipocentro;
        private double valorMagnitud;

        // Atributos referenciales
        private ClasificacionSismo clasificacion;
        private MagnitudRichter magnitud;
        private OrigenDeGeneracion origenGeneracion;
        private AlcanceSismo alcanceSismo;
        private Estado estadoActual;
        public List<CambioEstado> cambioEstado;
        public List<SerieTemporal> serieTemporal { get; set; } = new List<SerieTemporal>();

        public EventoSismico(
            DateTime fechaHoraOcurrencia,
            DateTime fechaHoraFin,
            double latitudEpicentro,
            double longitudEpicentro,
            double latitudHipocentro,
            double longitudHipocentro,
            double valorMagnitud,

            ClasificacionSismo clasificacion,
            MagnitudRichter magnitud,
            OrigenDeGeneracion origenGeneracion,
            AlcanceSismo alcanceSismo,
            Estado estadoActual,
            List<CambioEstado> cambioEstado,
            List<SerieTemporal> serieTemporal

            )
        {
            this.fechaHoraOcurrencia = fechaHoraOcurrencia;
            this.fechaHoraFin = fechaHoraFin;
            this.latitudEpicentro = latitudEpicentro;
            this.longitudEpicentro = longitudEpicentro;
            this.latitudHipocentro = latitudHipocentro;
            this.longitudHipocentro = longitudHipocentro;
            this.valorMagnitud = valorMagnitud;

            this.clasificacion = clasificacion;
            this.magnitud = magnitud;
            this.origenGeneracion = origenGeneracion;
            this.alcanceSismo = alcanceSismo;
            this.estadoActual = estadoActual;
            this.cambioEstado = cambioEstado;
            this.serieTemporal = serieTemporal;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Evento Sísmico:");
            sb.AppendLine($"  Fecha y Hora de Ocurrencia: {fechaHoraOcurrencia:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"  Fecha y Hora de Fin: {fechaHoraFin:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"  Epicentro: Latitud {latitudEpicentro:F6}, Longitud {longitudEpicentro:F6}");
            sb.AppendLine($"  Hipocentro: Latitud {latitudHipocentro:F6}, Longitud {longitudHipocentro:F6}");
            sb.AppendLine($"  Valor de Magnitud: {valorMagnitud:F1}");
            sb.AppendLine($"  Clasificación: {clasificacion?.Nombre ?? "Sin clasificar"}");
            sb.AppendLine($"  Origen de Generación: {origenGeneracion?.Nombre ?? "Sin origen"}");
            sb.AppendLine($"  Alcance: {alcanceSismo?.Nombre ?? "Sin alcance"}");
            sb.AppendLine($"  Estado Actual: {estadoActual?.NombreEstado ?? "Sin estado"}");
            sb.AppendLine($"  Cambios de Estado: {(cambioEstado != null ? cambioEstado.Count : 0)} cambios");

            foreach (var cambio in cambioEstado ?? new List<CambioEstado>())

                sb.AppendLine($"  Series Temporales: {(serieTemporal != null ? serieTemporal.Count : 0)} series");
            foreach (var serie in serieTemporal ?? new List<SerieTemporal>())
            {
                sb.AppendLine($"    - Serie desde {serie.FechaHoraInicioRegistroMuestras:yyyy-MM-dd HH:mm:ss} " +
                              $"hasta {serie.FechaHoraFinRegistro:yyyy-MM-dd HH:mm:ss}, " +
                              $"Condición Alarma: {serie.CondicionAlarma}, Frecuencia Muestreo: {serie.FrecuenciaMuestreo} Hz, " +
                              $"Muestras: {(serie.MuestrasSismicas != null ? serie.MuestrasSismicas.Count : 0)}");
            }

            return sb.ToString();
        }
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public ClasificacionSismo Clasificacion
        {
            get { return clasificacion; }
            set { clasificacion = value; }
        }

        public OrigenDeGeneracion OrigenGeneracion
        {
            get { return origenGeneracion; }
            set { origenGeneracion = value; }
        }
        public AlcanceSismo AlcanceSismo
        {
            get { return alcanceSismo; }
            set { alcanceSismo = value; }
        }

        public List<SerieTemporal> SerieTemporal => serieTemporal;

        public bool EsAutoDetectado()
        {
            return estadoActual.EsAutoDetectado();
        }

        public string TomarDatosPrincipales()
        {
            return $"Fecha: {fechaHoraOcurrencia:yyyy-MM-dd HH:mm:ss}| " +
                   $"Epicentro: ({latitudEpicentro:F6}, {longitudEpicentro:F6})| " +
                   $"Hipocentro: ({latitudHipocentro:F6}, {longitudHipocentro:F6})| " +
                   $"Magnitud: {valorMagnitud:F1}";
        }

        public DateTime FechaHoraOcurrencia
        {
            get { return fechaHoraOcurrencia; }
            set { fechaHoraOcurrencia = value; }
        }

        public void BloquearEvento(DateTime fh, Empleado empleado)
        {
            estadoActual.Revisar(fh, empleado, cambioEstado.ToArray(), this);
        }

        public void cambiarEstado(Estado estado, CambioEstado cambio)
        {
            estadoActual = estado;
            cambioEstado.Add(cambio);

            // Persistir en la base de datos
            EventoSismicoDAO eventoDAO = new EventoSismicoDAO();
            CambioEstadoDAO cambioDAO = new CambioEstadoDAO();

            // 1. Actualizar el estado actual del evento
            eventoDAO.ActualizarEstado(this.Id, estado.NombreEstado);

            // 2. Insertar el nuevo cambio de estado
            int? empleadoId = cambio.Responsable?.Id;
            cambioDAO.Insertar(cambio, this.Id, empleadoId);
        }

        public void rechazar(DateTime fh, Empleado usuarioLogueado)
        {
            estadoActual.Rechazar(fh, usuarioLogueado, cambioEstado.ToArray(), this);
        }

        //public void FinalizarUltimoCambioEstado()
        //{
        //    foreach (var cambioEstado in cambioEstado)
        //    {
        //        if (cambioEstado.EsEstadoActual())
        //        {
        //            cambioEstado.FechaHoraFin = DateTime.Now;
        //            break;
        //        }
        //    }
        //}

        //public void CrearCambioEstado(Estado estado, Empleado empleado)
        //{
        //    CambioEstado nuevoCambio = new CambioEstado(DateTime.Now, estado, empleado);
        //    cambioEstado.Add(nuevoCambio);
        //    estadoActual = estado;

        //    Console.WriteLine($"Cambio de estado creado: {nuevoCambio}");
        //}

        public (string Alcance, string Clasificacion, string Origen) BuscarDatosSismicosEventoSelec()
        {
            var alcanceEventoSelec = AlcanceSismo?.Nombre ?? "Sin alcance";
            var clasificacionEventoSelec = Clasificacion?.Nombre ?? "Sin clasificar";
            var origenEventoSelec = OrigenGeneracion?.Nombre ?? "Sin origen";

            return (alcanceEventoSelec, clasificacionEventoSelec, origenEventoSelec);
        }


        public DateTime FechaHoraFin
        {
            get { return fechaHoraFin; }
            set { fechaHoraFin = value; }
        }

        public double LatitudEpicentro
        {
            get { return latitudEpicentro; }
            set { latitudEpicentro = value; }
        }

        public double LongitudEpicentro
        {
            get { return longitudEpicentro; }
            set { longitudEpicentro = value; }
        }

        public double LatitudHipocentro
        {
            get { return latitudHipocentro; }
            set { latitudHipocentro = value; }
        }

        public double LongitudHipocentro
        {
            get { return longitudHipocentro; }
            set { longitudHipocentro = value; }
        }

        public double ValorMagnitud
        {
            get { return valorMagnitud; }
            set { valorMagnitud = value; }
        }

        public MagnitudRichter Magnitud
        {
            get { return magnitud; }
            set { magnitud = value; }
        }

        public Estado EstadoActual
        {
            get { return estadoActual; }
            set { estadoActual = value; }
        }

        public List<CambioEstado> CambiosEstado
        {
            get { return cambioEstado; }
            set { cambioEstado = value; }
        }

        public List<SerieTemporal> SeriesTemporales
        {
            get { return serieTemporal; }
            set { serieTemporal = value; }
        }

        
        // public string BuscarDatosSeriesTemporalesEvento()
        // {
        //     var sb = new StringBuilder();
        //     foreach (var serie in serieTemporal)
        //     {
        //         sb.AppendLine(serie.GetDatos());
        //     }
        //     return sb.ToString();
        // }

        //public void ActualizarEstado(Estado estadoRechazado, Empleado empleadoLogueado)
        //{
        //    foreach (var cambioEstado in cambioEstado)
        //    {
        //        if (cambioEstado.EsEstadoActual())
        //        {
        //            cambioEstado.FechaHoraFin = DateTime.Now;
        //            break;
        //        }
        //    }

        //    CrearCambioEstado(estadoRechazado, empleadoLogueado);
        //}
    }
}
