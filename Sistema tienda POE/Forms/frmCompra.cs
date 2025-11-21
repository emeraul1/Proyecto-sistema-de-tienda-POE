using Microsoft.Data.SqlClient;
using Sistema_tienda_POE.Clases;
using Sistema_tienda_POE.Repositorios;
using System;
using System.Data;
using System.Windows.Forms;
using System.Globalization;
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
            // Esto ayuda a evitar el error de NullReferenceException en el bucle de registro.
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
            ventana.ShowDialog();

            if (ventana.IdProveedorSeleccionado > 0)
            {
                idProveedorSeleccionado = ventana.IdProveedorSeleccionado;
                txtNombreProveedor.Text = ventana.NombreProveedorSeleccionado;
            }
        }


        // buscar producto por codigo
        private void BuscarProducto()
        {
            // Limpiamos la información del producto antes de buscar
            LimpiarProducto();

            if (string.IsNullOrWhiteSpace(txtCodigoBarra.Text))
            {
                MessageBox.Show("Ingrese un código de barras.");
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
                        // Línea que te da error, revisada y limpia
                        idProductoSeleccionado = (int)dr["IdProducto"];
                        nombreProductoSeleccionado = dr["Nombre"].ToString(); // ⬅️ AHORA LIMPIA

                        // Formato de costo para decimales
                        decimal costo = (decimal)dr["Costo"];
                        txtCostoUnitario.Text = costo.ToString("N2");

                        txtCantidad.ReadOnly = false;
                        txtCantidad.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Producto no encontrado.");
                        txtCodigoBarra.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al buscar el producto: " + ex.Message);
                }
            }
        }

        // Permitir solo dígitos en Cantidad
        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        // Permitir dígitos, punto, coma y control en Costo
        private void txtCostoUnitario_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir dígitos, la tecla de control (como backspace) y separadores decimales
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ','))
            {
                e.Handled = true;
            }
            // Controlar que solo haya un punto o coma decimal
            if ((e.KeyChar == '.') || (e.KeyChar == ','))
            {
                string separador = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                if (((TextBox)sender).Text.Contains(separador))
                {
                    e.Handled = true;
                }
                // Si el carácter presionado no es el separador actual, también lo bloqueamos si ya existe uno.
                else if (e.KeyChar.ToString() != separador)
                {
                    e.Handled = true;
                }
            }
        }

        // agrega el prodcto al detalle
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (idProductoSeleccionado == 0)
            {
                MessageBox.Show("Debe seleccionar un producto (escriba el código y presione Enter).");
                return;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Ingrese una cantidad válida y mayor a cero.");
                txtCantidad.Focus();
                return;
            }

            // Intenta parsear el costo de forma segura, usando la cultura actual del sistema
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
                subtotal // Se mantienen como decimal para calculo interno
            );

            ActualizarTotal();
            LimpiarProducto(); // Limpia los campos de producto para la siguiente adición
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
                // La verificación de IsNewRow no es necesaria si AllowUserToAddRows = false, pero es una buena práctica
                if (fila.IsNewRow) continue;

                // Verifica que el valor exista y sea convertible
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
            txtCodigoBarra.Text = "";
            txtCostoUnitario.Text = "";
            txtCantidad.Text = "1";
            txtCantidad.ReadOnly = true;
            txtCodigoBarra.Focus();
        }

        private void Limpiar()
        {
            LimpiarProducto(); // limpia la sección del producto

            // limpia la sección del proveedor y detalle
            idProveedorSeleccionado = 0;
            txtNombreProveedor.Text = "";
            dgvDetalle.Rows.Clear();
            lbTextoTotal.Text = "Total: $0.00";
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

            // Esta validación debe contemplar que la única fila puede ser la vacía de adición
            if (dgvDetalle.Rows.Count == 0 || (dgvDetalle.Rows.Count == 1 && dgvDetalle.AllowUserToAddRows))
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

                    // Se inserta la cabecera de la compra.
                    int idCompra = repo.InsertCompra(compra);

                    // Se insertan los detalles y se actualiza el stock.
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

                    // Si todo fue bien, confirmar la transacción.
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

        private void txtCodigoBarra_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtNombreProveedor_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCantidad_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCostoUnitario_TextChanged(object sender, EventArgs e)
        {

        }
    }
}