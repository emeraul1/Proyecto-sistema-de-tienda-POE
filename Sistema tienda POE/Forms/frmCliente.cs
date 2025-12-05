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
using RepoDb;
using Sistema_tienda_POE.Clases;

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

        private int clienteSeleccionadoId = 0;

        private void frmCliente_Load(object sender, EventArgs e)
        {
            cargarCliente();
        }

        private void cargarCliente()
        {
            using (var uow = new UnitOfwork(_connectionString))
            {
                var clientes = uow.Cliente.GetByEstado(true).ToList();
                dgvCliente.DataSource = clientes;
                dgvCliente.Columns["IdCliente"].Visible = false;
                dgvCliente.Columns["Estado"].Visible = false;
                dgvCliente.Columns["Nombres"].HeaderText = "Nombres";
                dgvCliente.Columns["Apellidos"].HeaderText = "Apellidos";
                dgvCliente.Columns["DUI"].HeaderText = "DUI";
                dgvCliente.Columns["Telefono"].HeaderText = "Teléfono";
                dgvCliente.Columns["Direccion"].HeaderText = "Dirección";
                dgvCliente.Refresh();
            }
        }
        private void LimpiarControles()
        {
            txtApellidos.Clear();
            txtDUI.Clear();
            txtDireccion.Clear();
            txtNombres.Clear();
            txtTelefono.Clear();
            clienteSeleccionadoId = 0;

        }
        private void btnAgregar_Click(object sender, EventArgs e)
        {

            if (clienteSeleccionadoId > 0)
            {
                MessageBox.Show("Este cliente solo se puede Actualizar");
                LimpiarControles();
                return;
            }

            if (string.IsNullOrEmpty(txtNombres.Text) || string.IsNullOrEmpty(txtApellidos.Text) || string.IsNullOrEmpty(txtTelefono.Text) ||
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

            MessageBox.Show("Cliente agregado correctamente.");
            LimpiarControles();
            cargarCliente();
            

        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRemover_Click(object sender, EventArgs e)
        {
            if (clienteSeleccionadoId == 0)
            {
                MessageBox.Show("Por favor, seleccione un cliente para remover.");
                return;
            }

            var confirmResult = MessageBox.Show("¿Está seguro que desea remover este cliente?", "Confirmar Remoción", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            var cliente = new Clases.Cliente
            {
                IdCliente = clienteSeleccionadoId,
                Estado = false
            };

            var camposActualizar = new List<Field>();
            camposActualizar.Add(new Field("Estado"));

            using (var uow = new UnitOfwork(_connectionString))
            {
                if (camposActualizar.Any())
                {
                    uow.Cliente.Update(cliente, camposActualizar);
                    uow.Commit();
                    MessageBox.Show("Cliente eliminado correctamente");
                    LimpiarControles();
                    clienteSeleccionadoId = 0;
                    cargarCliente();
                }
                else
                {
                    MessageBox.Show("No se completo la operación");
                }
            }
        }

        private void dgvCliente_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var cliente = (Cliente)dgvCliente.Rows[e.RowIndex].DataBoundItem;

                using (var formEditar = new frmClienteCmd(cliente.IdCliente, _connectionString))
                {
                    var result = formEditar.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        cargarCliente(); // recarga el grid si hubo cambios
                    }
                }
            }
        }

        private void dgvCliente_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var cliente = (Cliente)dgvCliente.Rows[e.RowIndex].DataBoundItem;
                clienteSeleccionadoId = cliente.IdCliente;
                txtNombres.Text = cliente.Nombres;
                txtApellidos.Text = cliente.Apellidos;
                txtTelefono.Text = cliente.Telefono;
                txtDUI.Text = cliente.DUI;
                txtDireccion.Text = cliente.Direccion;

            }
        }

        private void txtDUI_KeyPress(object sender, KeyPressEventArgs e)
        {
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

            txt.SelectionStart = txt.Text.Length;
            txt.TextChanged += txtDUI_TextChanged;
        }
    }
}
