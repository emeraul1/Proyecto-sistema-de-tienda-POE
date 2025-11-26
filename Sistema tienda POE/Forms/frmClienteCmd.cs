using RepoDb;
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
    public partial class frmClienteCmd : Form
    {
        private readonly string _connectionString;
        private readonly int _clienteId;
        public frmClienteCmd(int clienteId, string connectionString)
        {
            InitializeComponent();
            _clienteId = clienteId;
            _connectionString = connectionString;
        }

        private void frmClienteCmd_Load(object sender, EventArgs e)
        {
            using (var uow = new UoW.UnitOfwork(_connectionString))
            {
                var cliente = uow.Cliente.GetById(_clienteId);
                if (cliente != null)
                {
                    txtNombres.Text = cliente.Nombres;
                    txtApellidos.Text = cliente.Apellidos;
                    txtTelefono.Text = cliente.Telefono;
                    txtDUI.Text = cliente.DUI;
                    txtDireccion.Text = cliente.Direccion;
                }
                else
                {
                    MessageBox.Show("Cliente no encontrado.");
                    this.Close();
                }
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNombres.Text) || string.IsNullOrEmpty(txtApellidos.Text) || string.IsNullOrEmpty(txtTelefono.Text) ||
               string.IsNullOrWhiteSpace(txtDUI.Text))
            {
                MessageBox.Show("Por favor, completa todos los campos.");
                return;
            }

            var clienteActualizado = new Clases.Cliente
            {
                IdCliente = _clienteId,
                Nombres = txtNombres.Text.Trim(),
                Apellidos = txtApellidos.Text.Trim(),
                Telefono = txtTelefono.Text.Trim(),
                DUI = txtDUI.Text.Trim(),
                Direccion = txtDireccion.Text.Trim()
            };

            var camposActualizar = new List<Field>
            {
                new Field("Nombres"),
                new Field("Apellidos"),
                new Field("Telefono"),
                new Field("DUI"),
                new Field("Direccion"),
                new Field("Estado")

            };

            using (var uow = new UoW.UnitOfwork(_connectionString))
            {
                uow.Cliente.Update(clienteActualizado, camposActualizar);
                uow.Commit();
                MessageBox.Show("Cliente actualizado exitosamente.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
