using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_tienda_POE.Forms
{
    public partial class frmPrincipalAdministrador : Form
    {

        public frmPrincipalAdministrador()
        {
            InitializeComponent();
        }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            var frmUsuarios = new frmUsuarios();
            
            frmUsuarios.Show();

        }

        private void btnCerarSesion_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tbnProductos_Click(object sender, EventArgs e)
        {
            var frmProductos = new frmProductos();
            frmProductos.Show();
        }
    }
}
