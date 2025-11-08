using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAI_V2.entidades
{
    internal class Sismografo
    {
        // Atributos
        private DateTime fechaAdquisicion;
        private string identificadorSismografo { get; set; }
        private string nroSerie;

        // Atributos referenciales
        private EstacionSismologica estacionSismologica;
        private List<SerieTemporal> serieTemporal;
        // private Estado estadoActual;
        // private List<CambioEstado> cambioEstados;

        public Sismografo(
            DateTime fechaAdquisicion,
            string identificadorSismografo,
            string nroSerie,
            EstacionSismologica estacionSismologica,
            List<SerieTemporal> serieTemporal
            // Estado estadoActual,
            // List<CambioEstado> cambioEstados
            )
        {
            this.fechaAdquisicion = fechaAdquisicion;
            this.identificadorSismografo = identificadorSismografo;
            this.nroSerie = nroSerie;
            this.estacionSismologica = estacionSismologica;
            this.serieTemporal = serieTemporal;
            // this.estadoActual = estadoActual;
            // this.cambioEstados = cambioEstados;
        }

        public string IdentificadorSismografo => identificadorSismografo;
        public EstacionSismologica EstacionSismologica => estacionSismologica;
        public List<SerieTemporal> SerieTemporal => serieTemporal;

        public List<SerieTemporal> ObtenerSeriesRelevantes(IEnumerable<SerieTemporal> seriesEvento)
        {
            return SerieTemporal
                .Where(st => seriesEvento.Contains(st))
                .ToList();
        }

        public String TomarNombreEstacion()
        {
            return EstacionSismologica.Nombre;
        }

        public override string ToString()
        {
            return $"Sismógrafo: {identificadorSismografo}, Estación: {estacionSismologica?.Nombre ?? "Sin estación"}";
        }
    }
}
