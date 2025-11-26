using Sistema_tienda_POE.UoW;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace Sistema_tienda_POE.Forms
{
    public partial class frmCliente : Form
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["conexionBD"].ConnectionString;
        public frmCliente(string connectionString)
        {
            InitializeComponent();
            _connectionString = connectionString;
        }

       

        private void frmCliente_Load(object sender, EventArgs e)
        {
            cargarCliente();
        }

        private void cargarCliente()
        {
            // Aquí puedes cargar datos iniciales si es necesario
            using (var uow = new UnitOfwork(_connectionString))
            {
                var clientes = uow.Cliente.GetAll();
                // Si necesitas mostrar los clientes en algún control, puedes hacerlo aquí
                dgvCliente.DataSource = clientes;
                dgvCliente.Columns["IdCliente"].Visible = false;
                //nombre 
                dgvCliente.Columns["Nombres"].HeaderText = "Nombres";
                //apellido
                dgvCliente.Columns["Apellidos"].HeaderText = "Apellidos";
                //dui
                dgvCliente.Columns["DUI"].HeaderText = "DUI";
                //telefono
                dgvCliente.Columns["Telefono"].HeaderText = "Teléfono";
                //direccion
                dgvCliente.Columns["Direccion"].HeaderText = "Dirección";
                //estado
                dgvCliente.Columns["Estado"].HeaderText = "Estado";
                dgvCliente.Refresh();
            }
        }
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtNombres.Text) || string.IsNullOrEmpty(txtApellidos.Text) || string.IsNullOrEmpty(txtTelefono.Text) ||
                string.IsNullOrWhiteSpace(txtDUI.Text))
            {
                MessageBox.Show("Por favor, completa todos los campos.");
                return;
            }

            var nuevoCliente = new Clases.Cliente
            {
                Nombres = txtNombres.Text.Trim(),
                Apellidos = txtApellidos.Text.Trim(),
                Telefono = txtTelefono.Text.Trim(),
                DUI = txtDUI.Text.Trim(),
                Direccion = txtDireccion.Text.Trim(),
                Estado = true
            };
            using (var uow = new UnitOfwork(_connectionString))
            {
                uow.Cliente.Insert(nuevoCliente);
                uow.Commit();
            }
            MessageBox.Show("Cliente agregado exitosamente.");
            this.Close();

        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
