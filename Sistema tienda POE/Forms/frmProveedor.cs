using Microsoft.Data.SqlClient;
using Sistema_tienda_POE.Clases;
using Sistema_tienda_POE.Repositorios;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Sistema_tienda_POE.Forms
{
    public partial class frmProveedor : Form
    {
        private readonly ProveedorRepository _repo;

        public frmProveedor()
        {
            InitializeComponent();

            _repo = new ProveedorRepository(ConexionBD.ObtenerConexion());
        }

        private void frmProveedor_Load(object sender, EventArgs e)
        {
            CargarProveedores();
            LimpiarCampos();
        }

        private void CargarProveedores()
        {
            try
            {
                // Leer solo los proveedores activos para la vista principal
                dgvProveedores.DataSource = _repo.GetByEstado(true).ToList();

                // Ocultar la columna de ID si no es necesaria para el usuario
                if (dgvProveedores.Columns.Contains("IdProveedor"))
                {
                    dgvProveedores.Columns["IdProveedor"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar proveedores: {ex.Message}", "Error de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Validaciones basicas
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtNIT.Text))
            {
                MessageBox.Show("El Nombre y el NIT son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //  Crear objeto Proveedor
            Proveedor p = new Proveedor
            {
                IdProveedor = int.TryParse(txtIdProveedor.Text, out int id) ? id : 0,
                Nombre = txtNombre.Text,
                NIT = txtNIT.Text,
                Telefono = txtTelefono.Text,
                Direccion = txtDireccion.Text,
                Estado = chkEstado.Checked // Captura el estado del CheckBox
            };

            try
            {
                if (p.IdProveedor == 0)
                {
                    // INSERT (Crear)
                    _repo.Insert(p);
                    MessageBox.Show("Proveedor registrado con éxito.", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // UPDATE (Actualizar)
                    _repo.Update(p);
                    MessageBox.Show("Proveedor actualizado con éxito.", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                LimpiarCampos();
                CargarProveedores(); // Refrescar la lista
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el proveedor: {ex.Message}", "Error de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtIdProveedor.Text, out int idProveedor) && idProveedor > 0)
            {
                if (MessageBox.Show("¿Está seguro de inactivar este proveedor?", "Confirmar Eliminación Lógica", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        // D (DELETE) 
                        _repo.EliminarLogico(idProveedor);
                        MessageBox.Show("Proveedor inactivado correctamente.", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCampos();
                        CargarProveedores(); // Refrescar la lista
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar (lógicamente) el proveedor: {ex.Message}", "Error de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar un proveedor para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }


        private void LimpiarCampos()
        {
            txtIdProveedor.Text = "0"; 
            txtNombre.Clear();
            txtNIT.Clear();
            txtTelefono.Clear();
            txtDireccion.Clear();
            chkEstado.Checked = true;
            txtNombre.Focus();
        }

        private void dgvProveedores_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Rellenar los campos al seleccionar una fila
                DataGridViewRow fila = dgvProveedores.Rows[e.RowIndex];

                txtIdProveedor.Text = fila.Cells["IdProveedor"].Value.ToString();
                txtNombre.Text = fila.Cells["Nombre"].Value.ToString();
                txtNIT.Text = fila.Cells["NIT"].Value?.ToString();
                txtTelefono.Text = fila.Cells["Telefono"].Value?.ToString();
                txtDireccion.Text = fila.Cells["Direccion"].Value?.ToString();

                // Mapear el valor bool 'Estado' del DGV al CheckBox
                if (fila.Cells["Estado"].Value != null)
                {
                    chkEstado.Checked = (bool)fila.Cells["Estado"].Value;
                }

                btnGuardar.Text = "Actualizar"; // Cambia el texto del botón al modo edición
            }
        }

        // Sobreescribir el botón de Guardar al limpiar
        private void LimpiarCampos_AfterUpdate()
        {
            LimpiarCampos();
            btnGuardar.Text = "Guardar";
        }
    }
}
