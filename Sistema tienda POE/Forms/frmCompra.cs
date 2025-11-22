using Microsoft.Data.SqlClient;
using Sistema_tienda_POE.Clases;
using Sistema_tienda_POE.Repositorios;
using System;
using System.Data;
using System.Windows.Forms;
using System.Globalization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Sistema_tienda_POE.Forms
{
    public partial class frmCompra : Form
    {
        private int idProveedorSeleccionado = 0;
        private int idProductoSeleccionado = 0;
        private string nombreProductoSeleccionado = "";

        public frmCompra()
        {
            InitializeComponent();
        }

        private void frmCompra_Load(object sender, EventArgs e)
        {
            ConfigurarTabla();
            Limpiar();
            // Esto asegura que el DataGrid no tenga una fila vacia extra
            dgvDetalle.AllowUserToAddRows = false;
        }

        private void ConfigurarTabla()
        {
            dgvDetalle.Columns.Clear();
            dgvDetalle.Columns.Add("IdProducto", "IdProducto");
            dgvDetalle.Columns.Add("Codigo", "Código");
            dgvDetalle.Columns.Add("Nombre", "Nombre");
            dgvDetalle.Columns.Add("Cantidad", "Cantidad");
            dgvDetalle.Columns.Add("Costo", "Costo");
            dgvDetalle.Columns.Add("Subtotal", "Subtotal");

            dgvDetalle.Columns["IdProducto"].Visible = false;
        }


        // buscar proveedor
        private void btnBuscarProveedor_Click(object sender, EventArgs e)
        {
            frmBuscarProveedor ventana = new frmBuscarProveedor();
            // Abrimos el formulario de busqueda como dialogo para pausar este formulario
            if (ventana.ShowDialog() == DialogResult.OK)
            {
                if (ventana.IdProveedorSeleccionado > 0)
                {
                    idProveedorSeleccionado = ventana.IdProveedorSeleccionado;
                    txtNombreProveedor.Text = ventana.NombreProveedorSeleccionado;
                }
            }
        }


        private void txtCodigoBarra_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigoBarra.Text))
            {
                LimpiarProducto();
                return;
            }

            
            if (txtCodigoBarra.Text.Length == 5)
            {
                BuscarProducto();
            }
            // aseguramos que los campos de detalle esten limpios
            else if (txtCodigoBarra.Text.Length < 5)
            {
                LimpiarProducto();
            }
        }

        // buscar producto por codigo
        private void BuscarProducto()
        {
           
            idProductoSeleccionado = 0;
            nombreProductoSeleccionado = "";
            txtCostoUnitario.Text = "";
            txtCantidad.Text = "1";
            txtCantidad.ReadOnly = false;

            //ValidaciOn de la caja de texto
            if (string.IsNullOrWhiteSpace(txtCodigoBarra.Text))
            {
                return;
            }

            using (Microsoft.Data.SqlClient.SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                try
                {
                    conexion.Open();

                    // Consulta SQL
                    SqlCommand cmd = new SqlCommand(
                        "SELECT IdProducto, Nombre, Costo FROM Producto WHERE CodigoBarras = @codigo",
                        conexion);

                    cmd.Parameters.AddWithValue("@codigo", txtCodigoBarra.Text);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        //Asignar valores
                        idProductoSeleccionado = (int)dr["IdProducto"];
                        nombreProductoSeleccionado = dr["Nombre"].ToString();

                        decimal costo = (decimal)dr["Costo"];
                        txtCostoUnitario.Text = costo.ToString("N2");
                        txtCantidad.Focus();

                    txtCantidad.ReadOnly = false; // Habilitar la cantidad
                        txtCantidad.Focus(); // Mueve el foco a la cantidad para el usuario
                    }
                    else
                    {
                        // Producto no encontrado
                        MessageBox.Show("Producto no encontrado.", "Búsqueda Fallida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCodigoBarra.Text = ""; // Limpiamos el código para que pueda intentar de nuevo
                        txtCodigoBarra.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al buscar el producto. Detalle: " + ex.Message, "Error de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Permitir solo digitos y control en Cantidad
        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        // Permitir digitos, punto, coma en Costo
        private void txtCostoUnitario_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permite dígitos y teclas de control
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ','))
            {
                e.Handled = true;
            }
            // Controla que solo haya un separador decimal
            if ((e.KeyChar == '.') || (e.KeyChar == ','))
            {
                string separador = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                if (((System.Windows.Forms.TextBox)sender).Text.Contains(separador))
                {
                    e.Handled = true;
                }
                else if (e.KeyChar.ToString() != separador)
                {
                    e.Handled = true; // Bloquea el separador incorrecto si ya existe uno.
                }
            }
        }

        // agregar producto al detalle
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (idProveedorSeleccionado == 0)
            {
                MessageBox.Show("Debe seleccionar un proveedor primero.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (idProductoSeleccionado == 0)
            {
                MessageBox.Show("Debe seleccionar un producto (escriba el código).", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Ingrese una cantidad válida y mayor a cero.");
                txtCantidad.Focus();
                return;
            }

            if (!decimal.TryParse(txtCostoUnitario.Text, NumberStyles.Currency | NumberStyles.Number, CultureInfo.CurrentCulture, out decimal costo) || costo <= 0)
            {
                MessageBox.Show("Ingrese un costo unitario válido y mayor a cero.");
                txtCostoUnitario.Focus();
                return;
            }

            decimal subtotal = cantidad * costo;

            // Agrega la fila
            dgvDetalle.Rows.Add(
                idProductoSeleccionado,
                txtCodigoBarra.Text,
                nombreProductoSeleccionado,
                cantidad,
                costo,
                subtotal // Se mantienen como decimal para cálculo interno
            );

            ActualizarTotal();
            LimpiarProducto(); // Limpia los campos de detalle

            //Limpiar código de barras para el siguiente producto
            txtCodigoBarra.Text = "";
            txtCodigoBarra.Focus();
        }

        // quitar producto
        private void btnQuitar_Click(object sender, EventArgs e)
        {
            if (dgvDetalle.SelectedRows.Count > 0)
            {
                dgvDetalle.Rows.RemoveAt(dgvDetalle.SelectedRows[0].Index);
                ActualizarTotal();
            }
            else
            {
                MessageBox.Show("Seleccione una fila para quitar.");
            }
        }

        private void ActualizarTotal()
        {
            decimal total = ObtenerTotal();
            // Formato para mostrar el total en la etiqueta
            lbTextoTotal.Text = $"Total: ${total:N2}";
        }

        private decimal ObtenerTotal()
        {
            decimal total = 0;

            foreach (DataGridViewRow fila in dgvDetalle.Rows)
            {
                if (fila.IsNewRow) continue;

                if (fila.Cells["Subtotal"].Value != null)
                {
                    total += Convert.ToDecimal(fila.Cells["Subtotal"].Value);
                }
            }
            return total;
        }

        private void LimpiarProducto()
        {
            idProductoSeleccionado = 0;
            nombreProductoSeleccionado = "";
            txtCostoUnitario.Text = "";
            txtCantidad.Text = "";
            txtCantidad.ReadOnly = false;
        }
        private void Limpiar()
        {
            LimpiarProducto(); // Limpia campos de detalle de producto

            // limpia la sección del proveedor y detalle
            idProveedorSeleccionado = 0;
            txtNombreProveedor.Text = "";
            dgvDetalle.Rows.Clear();
            lbTextoTotal.Text = "Total: $0.00";

            //limpia código de barras y foco
            txtCodigoBarra.Text = "";
            txtCodigoBarra.Focus();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        // registar compra
        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (idProveedorSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un proveedor.");
                return;
            }

            if (dgvDetalle.Rows.Count == 0)
            {
                MessageBox.Show("Agregue productos al detalle de la compra.");
                return;
            }

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                conexion.Open();
                SqlTransaction transaccion = conexion.BeginTransaction();

                try
                {
                    CompraRepository repo = new CompraRepository(conexion, transaccion);

                    // Creación del objeto Compra
                    Compra compra = new Compra()
                    {
                        IdProveedor = idProveedorSeleccionado,
                        IdUsuario = 4, // IdUsuario fijo.
                        Fecha = DateTime.Now,
                        Subtotal = ObtenerTotal(),
                        Descuento = 0,
                        Impuesto = 0,
                        Total = ObtenerTotal(),
                        Estado = true,
                        Observacion = ""
                    };

                    // Se inserta la cabecera de la compra
                    int idCompra = repo.InsertCompra(compra);

                    // Se insertan los detalles y se actualiza el stock
                    foreach (DataGridViewRow fila in dgvDetalle.Rows)
                    {
                        // Ignorar la fila de adición
                        if (fila.IsNewRow) continue;

                        DetalleCompra det = new DetalleCompra()
                        {
                            IdCompra = idCompra,
                            IdProducto = (int)fila.Cells["IdProducto"].Value,
                            Cantidad = (int)fila.Cells["Cantidad"].Value,
                            CostoUnitario = Convert.ToDecimal(fila.Cells["Costo"].Value),
                            Subtotal = Convert.ToDecimal(fila.Cells["Subtotal"].Value)
                        };

                        repo.InsertDetalle(det);
                        repo.ActualizarStock(det.IdProducto, det.Cantidad);
                    }

                    // confirmar la transaccion
                    transaccion.Commit();
                    MessageBox.Show("Compra registrada con éxito.");
                    Limpiar();
                }
                catch (Exception ex)
                {
                    // Si algo falla, revertir todo.
                    transaccion.Rollback();
                    MessageBox.Show($"Error al registrar la compra. La transacción fue revertida. Detalle: {ex.Message}");
                }
            }
        }

        private void txtNombreProveedor_TextChanged(object sender, EventArgs e) { }
        private void txtCantidad_TextChanged(object sender, EventArgs e) { }
        private void txtCostoUnitario_TextChanged(object sender, EventArgs e) { }
    }
}
