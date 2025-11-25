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
    public partial class frmPrincipalAdministrador : Form
    {
        private readonly string _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conexionBD"].ConnectionString;

        public frmPrincipalAdministrador()
        {
            InitializeComponent();
        }

        // Gestion de Usuarios
        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            var frmUsuarios = new frmUsuarios();
            frmUsuarios.Show();
        }

        // Gestión de Productos
        private void tbnProductos_Click(object sender, EventArgs e)
        {
            var frmProductos = new frmProductos();
            frmProductos.Show();
        }

        // Registro de Venta
        private void btnVender_Click(object sender, EventArgs e)
        {
            var frm = new frmVenta(_connectionString, GlobalUsuario.UsuarioActual.IdUsuario);
            frm.Show();
        }

        /// Abre el formulario de Registro de Compra.
        private void btnCompras_Click(object sender, EventArgs e)
        {
            // Instancia y muestra el formulario de compra.
            var frmCompras = new frmCompra();
            frmCompras.Show();
        }

        private void btnCerarSesion_Click(object sender, EventArgs e)
        
        {
        
            // 1. Preguntar confirmación
            if (MessageBox.Show("¿Está seguro que desea cerrar la sesión?", "Confirmar Cierre", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // 2. Detener el formulario principal
                this.Close();

                // 3. Reiniciar la aplicación desde el principio (volverá a ejecutar Program.Main())
                Application.Restart();
            
        }
            




        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
        }

        private void btnCompras_Click_1(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            {
                var frmProveedor = new frmProveedor();
                frmProveedor.Show();
            }

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnReporteVentas_Click(object sender, EventArgs e)
        {
            using (var frm = new frmReporteVentas())
            {
                frm.ShowDialog();
            }
        }
    }
}