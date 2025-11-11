using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAI_V2.entidades
{
    public class CambioEstado
    {
        // Atributos
        private DateTime? fechaHoraFin;
        private DateTime fechaHoraInicio;

        // Atributos referenciales
        private Estado estado;
        private Empleado responsableInspeccion;

        public int Id { get; private set; }

        public CambioEstado(DateTime fechaHoraInicio, Estado estado, Empleado responsableInspeccion)
        {
            this.fechaHoraInicio = fechaHoraInicio;
            this.estado = estado;
            this.responsableInspeccion = responsableInspeccion;
            fechaHoraFin = null; // Inicialmente no tiene fecha de fin
        }
        public CambioEstado(DateTime fechaHoraInicio, DateTime fechaHoraFin, Estado estado, Empleado responsableInspeccion)
        {
            this.fechaHoraInicio = fechaHoraInicio;
            this.estado = estado;
            this.responsableInspeccion = responsableInspeccion;
            this.fechaHoraFin = fechaHoraFin;
        }

        public DateTime FechaHoraInicio
        {
            get => fechaHoraInicio;
            set => fechaHoraInicio = value;
        }

        public Estado Estado
        {
            get => estado;
            set => estado = value;
        }

        public Empleado Responsable
        {
            get => responsableInspeccion;
            set => responsableInspeccion = value;
        }

        public DateTime? FechaHoraFin
        {
            get { return fechaHoraFin; }
            set { fechaHoraFin = value; }
        }

        public Empleado ResponsableInspeccion
        {
            get { return responsableInspeccion; }
            set { responsableInspeccion = value; }
        }

        public bool EsEstadoActual() =>
            fechaHoraFin == null;

        public override string ToString()
        {
            return $"CAMBIO DE ESTADO: Fecha y hora inicio: {fechaHoraInicio} - Fecha y hora fin: {fechaHoraFin} \nEstado: {estado.NombreEstado} - Responsable: {responsableInspeccion.ToString()}";
        }
    }
}
