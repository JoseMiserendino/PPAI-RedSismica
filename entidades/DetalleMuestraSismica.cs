using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAI_V2.entidades
{
    internal class DetalleMuestraSismica
    {
        // Atributos
        private double valor;

        // Atributos referenciales
        private TipoDeDato tipoDeDato;

        public DetalleMuestraSismica(double valor, TipoDeDato tipoDeDato)
        {
            this.valor = valor;
            this.tipoDeDato = tipoDeDato;
        }

        public double Valor
        {
            get { return valor; }
            set { valor = value; }
        }

        public (string Nombre, double Valor) GetDatos()
        {
            return (tipoDeDato.Denominacion, valor);
        }
    }
}
