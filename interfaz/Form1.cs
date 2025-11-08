using System;
using System.Windows.Forms;

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
    }
}
