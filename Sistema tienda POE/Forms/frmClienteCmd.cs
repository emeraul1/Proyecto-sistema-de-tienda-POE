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

        private void txtDUI_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir solo números y Backspace
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
                e.Handled = true;
        }

        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Validar que solo sea número
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Bloquea cualquier otra tecla
                MessageBox.Show("Solo se permiten números", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtDUI_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;

            // Pausar evento para evitar bucles
            txt.TextChanged -= txtDUI_TextChanged;

            // Solo números
            string limpio = new string(txt.Text.Where(char.IsDigit).ToArray());

            // Máximo 9 dígitos
            if (limpio.Length > 9)
                limpio = limpio.Substring(0, 9);

            // Insertar guion después de los primeros 8 dígitos
            if (limpio.Length >= 9)
                txt.Text = limpio.Insert(8, "-");
            else if (limpio.Length > 8)
                txt.Text = limpio.Insert(8, "-");
            else
                txt.Text = limpio;

            // Mantener cursor al final
            txt.SelectionStart = txt.Text.Length;

            // Reactivar evento
            txt.TextChanged += txtDUI_TextChanged;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
