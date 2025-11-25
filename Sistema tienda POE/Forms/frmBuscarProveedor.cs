using Microsoft.Data.SqlClient;
using Sistema_tienda_POE.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Sistema_tienda_POE.Forms
{
    public partial class frmBuscarProveedor : Form
    {
        public int IdProveedorSeleccionado { get; private set; }
        public string NombreProveedorSeleccionado { get; private set; }

        public frmBuscarProveedor()
        {
            InitializeComponent();
            this.IdProveedorSeleccionado = 0; // Inicializamos a 0
        }

        private void frmBuscarProveedor_Load(object sender, EventArgs e)
        {
            // Ocultar la columna de ID al inicio
            CargarProveedores();
            dgvProveedores.Columns["IdProveedor"].Visible = false;
        }

        private void CargarProveedores(string filtro = "")
        {
            List<Proveedor> lista = new List<Proveedor>();

            string consulta = "SELECT IdProveedor, Nombre FROM Proveedor";

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                consulta += " WHERE Nombre LIKE @filtro";
            }

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand(consulta, conexion);

                    if (!string.IsNullOrWhiteSpace(filtro))
                    {
                        cmd.Parameters.AddWithValue("@filtro", "%" + filtro + "%");
                    }

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Proveedor()
                            {
                                IdProveedor = (int)dr["IdProveedor"],
                                Nombre = dr["Nombre"].ToString()
                            });
                        }
                    }

                    dgvProveedores.DataSource = lista;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar proveedores: " + ex.Message, "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarProveedores(txtFiltro.Text);
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            if (dgvProveedores.SelectedRows.Count > 0)
            {
                // Leer los datos de la fila seleccionada
                DataGridViewRow fila = dgvProveedores.SelectedRows[0];

                this.IdProveedorSeleccionado = Convert.ToInt32(fila.Cells["IdProveedor"].Value);
                this.NombreProveedorSeleccionado = fila.Cells["Nombre"].Value.ToString();

                // Cierra la ventana y devuelve el control a frmCompra
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Debe seleccionar un proveedor de la lista.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Permite la selección con doble clic
        private void dgvProveedores_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnSeleccionar_Click(sender, e);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
