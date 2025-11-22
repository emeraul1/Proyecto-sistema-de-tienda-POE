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
    public partial class frmPrincipalCajero : Form
    {
        public frmPrincipalCajero()
        {
            InitializeComponent();
        }

        private void btnVender_Click(object sender, EventArgs e)
        {
            using (var frm = new frmReporteVentas())
            {
                frm.ShowDialog();
            }
        }

        private void btnCompras_Click(object sender, EventArgs e)
        {
            var frmCompras = new frmCompra();
            frmCompras.Show();
        }
    }
}
