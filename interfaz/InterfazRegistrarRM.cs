using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PPAI_V2
{
    public partial class InterfazRegistrarRM : Form
    {
        GestorRegistrarRM gestor;
        public InterfazRegistrarRM()
        {
            InitializeComponent();
            listViewEventos.MultiSelect = false;
            listViewEventos.FullRowSelect = true;
            listViewEventos.View = View.Details;

        }

        public void OpcRegistrarResultadoRM()
        {
            // Punto 5
            HabilitarPantalla();
            GestorRegistrarRM gestor = new GestorRegistrarRM(this);
            this.gestor = gestor;
            gestor.OpcRegistrarResultadoRM();
        }

        public void HabilitarPantalla()
        {
            this.Show();
        }

        public void PedirSeleccionEventoNoRevisado(List<string> eventos)
        {
            listViewEventos.Clear();
            listViewEventos.View = View.Details;
            listViewEventos.FullRowSelect = true;
            listViewEventos.Columns.Add("Fecha", 140);
            listViewEventos.Columns.Add("Epicentro", 140);
            listViewEventos.Columns.Add("Hipocentro", 140);
            listViewEventos.Columns.Add("Magnitud", 70);

            foreach (var datos in eventos)
            {
                string[] partes = datos.Split(new[] { "| " }, StringSplitOptions.None);

                string fecha = partes[0].Replace("Fecha: ", "");
                string epicentro = partes[1].Replace("Epicentro: ", "");
                string hipocentro = partes[2].Replace("Hipocentro: ", "");
                string magnitud = partes[3].Replace("Magnitud: ", "");

                var item = new ListViewItem(fecha);
                item.SubItems.Add(epicentro);
                item.SubItems.Add(hipocentro);
                item.SubItems.Add(magnitud);
                listViewEventos.Items.Add(item);
            }
        }

        private void TomarSelecEventoNoRevisado(object sender, EventArgs e)
        {
            if (listViewEventos.SelectedIndices.Count > 0)
            {
                gestor.TomarSelecEventoNoRevisado(listViewEventos.SelectedIndices[0]);
            }
        }

        internal void MostrarDatosSismicosEventoSelec(String datos)
        {
            panelDatosSismicos.Visible = true;
            labelDatosSismicos.Text = datos;
        }

        private void TomarConfirmacionEvento(object sender, EventArgs e)
        {
            gestor.TomarConfirmacionEvento();
        }

        internal void MostrarSismograma()
        {
            panelSismograma.Visible = true;
        }

        private void TomarConfirmacionSismograma(object sender, EventArgs e)
        {
            gestor.TomarConfirmacionSismograma();
        }

        internal void HabilitarOpcVisualizarMapa()
        {
            panelVisualizarMapa.Visible = true;
        }

        private void TomarSelecVisualizacionMapa(bool seleccion)
        {
            Console.WriteLine("=======================================================================");
            Console.WriteLine($"Selección de visualización de mapa: {seleccion}");
            gestor.TomarSelecVisualizacionMapa(seleccion);
        }

        private void RechazarVerMapa(object sender, EventArgs e)
        {
            TomarSelecVisualizacionMapa(false);
        }

        private void AceptarVerMapa(object sender, EventArgs e)
        {
            TomarSelecVisualizacionMapa(true);
        }

        internal void PedirModificacionEvento(String alcance, String clasificacion, String origen, List<String> alcances, List<String> clasificaciones, List<String> origenes)
        {
            panelModificacionEvento.Visible = true;

            comboBoxAlcances.Items.Clear();
            comboBoxClasificaciones.Items.Clear();
            comboBoxOrigenes.Items.Clear();
            comboBoxAlcances.Items.AddRange(alcances.ToArray());
            comboBoxClasificaciones.Items.AddRange(clasificaciones.ToArray());
            comboBoxOrigenes.Items.AddRange(origenes.ToArray());
            comboBoxAlcances.SelectedIndex = alcances.FindIndex(x => x == alcance);
            comboBoxClasificaciones.SelectedIndex = clasificaciones.FindIndex(x => x == clasificacion);
            comboBoxOrigenes.SelectedIndex = origenes.FindIndex(x => x == origen);

            // textBoxAlcance.Text = alcance;
            // textBoxClasificacion.Text = clasificacion;
            // textBoxOrigenGeneracion.Text = origen;
        }

        private void TomarModificacionEvento(object sender, EventArgs e)
        {
            var alcance = comboBoxAlcances.Items[comboBoxAlcances.SelectedIndex].ToString();
            var clasificacion = comboBoxClasificaciones.Items[comboBoxClasificaciones.SelectedIndex].ToString();
            var origen = comboBoxOrigenes.Items[comboBoxOrigenes.SelectedIndex].ToString();
            Console.WriteLine($"Alcance seleccionado: {alcance}");
            Console.WriteLine($"Clasificación seleccionada: {clasificacion}");
            Console.WriteLine($"Origen seleccionado: {origen}");
            gestor.TomarModificacionEvento(alcance, clasificacion, origen);
        }

        internal void PedirSelecOpcFinal(List<String> opciones)
        {
            panelOpcFinal.Visible = true;
            comboBoxOpcFinal.Items.Clear();
            comboBoxOpcFinal.Items.AddRange(opciones.ToArray());
            comboBoxOpcFinal.SelectedIndex = 0;
        }

        private void TomarSelecOpcFinal(object sender, EventArgs e)
        {
            string opcionSeleccionada = comboBoxOpcFinal.SelectedItem.ToString();
            gestor.TomarSelecOpcFinal(opcionSeleccionada);
        }

        internal void FinCU()
        {

            this.Close();
        }

        private void CancelarEjecucion(object sender, EventArgs e)
        {
            
            DialogResult result = MessageBox.Show("¿Está seguro de que desea cancelar el proceso?",
                                                  "Confirmar Cancelación",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Cerrar la ventana, lo que finaliza el caso de uso
                this.Close();
            }
        }
    }
}
