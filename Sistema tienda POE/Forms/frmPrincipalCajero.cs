using Sistema_tienda_POE.Clases;
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
        private readonly string _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;
        public frmPrincipalCajero()
        {
            InitializeComponent();
        }

        private void btnVender_Click(object sender, EventArgs e)
        {
            var frm = new frmVenta(_connectionString, GlobalUsuario.UsuarioActual.IdUsuario);
            frm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
