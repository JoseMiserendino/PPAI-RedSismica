using PPAI_V2.gestor;
using System;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace PPAI_V2
{
    public partial class interfazPrincipal : Form
    {
        public interfazPrincipal()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InterfazRegistrarRM interfazRegistrarRM = new InterfazRegistrarRM();
            interfazRegistrarRM.OpcRegistrarResultadoRM();
        }

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = ConexionDB.ObtenerConexion())
        //        {
        //            MessageBox.Show(
        //                "✓ Conexión exitosa a la base de datos RedSismica!\n\n" +
        //                "Estado: " + conn.State.ToString() + "\n" +
        //                "Servidor: " + conn.DataSource + "\n" +
        //                "Base de datos: " + conn.Database,
        //                "Conexión Exitosa",
        //                MessageBoxButtons.OK,
        //                MessageBoxIcon.Information
        //            );
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(
        //            "✗ Error al conectar con la base de datos:\n\n" + ex.Message,
        //            "Error de Conexión",
        //            MessageBoxButtons.OK,
        //            MessageBoxIcon.Error
        //        );
        //    }
        //}
    }
}
