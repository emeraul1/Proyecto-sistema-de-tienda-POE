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
using Sistema_tienda_POE.UoW;
using Sistema_tienda_POE.Clases;
using RepoDb;

namespace Sistema_tienda_POE.Forms
{
    public partial class frmVenta : Form
    {
        private readonly string _connectionString;
        private readonly int IdUsuario;
        public frmVenta(string connectionString, int idUsuario)
        {
            IdUsuario = idUsuario;
            _connectionString = connectionString;
            InitializeComponent();
        }

        private void frmVenta_Load(object sender, EventArgs e)
        {
            txtCantidad.Text = "1";
            CargarMetodosPago();
            ConfigurarGridVenta();
        }
        private void CargarMetodosPago()
        {
            using (var uow = new UnitOfwork(_connectionString))
            {
                var metodos = uow.MetodoPago.GetByEstado(true).ToList();
                cmbMetodoPago.DataSource = metodos;
                cmbMetodoPago.DisplayMember = "Nombre";
                cmbMetodoPago.ValueMember = "IdMetodoPago";
                cmbMetodoPago.SelectedIndex = -1;
            }
        }

        private void ConfigurarGridVenta()
        {
            dgvVenta.AutoGenerateColumns = false;
            dgvVenta.Columns.Clear();

            // IdProducto (oculto)
            var colId = new DataGridViewTextBoxColumn();
            colId.Name = "IdProducto";
            colId.HeaderText = "IdProducto";
            colId.Visible = false;
            dgvVenta.Columns.Add(colId);

            // Código de barras
            dgvVenta.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CodigoBarras",
                HeaderText = "Código"
            });

            // Nombre del producto
            dgvVenta.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NombreProducto",
                HeaderText = "Producto"
            });

            // Precio de venta
            dgvVenta.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PrecioVenta",
                HeaderText = "Precio"
            });

            // Cantidad
            dgvVenta.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cantidad",
                HeaderText = "Cant."
            });

            // Subtotal
            dgvVenta.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Subtotal",
                HeaderText = "Subtotal"
            });
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("Ingrese el código de barras.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Ingrese una cantidad válida.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Producto producto;
            using (var uow = new UnitOfwork(_connectionString))
            {
                producto = uow.Producto.GetByCodigoBarras(txtCodigo.Text.Trim());
            }

            if (producto == null)
            {
                MessageBox.Show("Producto no encontrado.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int cantidadd = int.Parse(txtCantidad.Text.Trim());
            decimal subtotal = cantidadd * producto.PrecioVenta;


            dgvVenta.Rows.Add(
                producto.IdProducto,
                producto.CodigoBarras,
                producto.Nombre,
                producto.PrecioVenta,
                cantidad,
                subtotal
            );


            CalacularTotal();
            Limpiar();
            txtCodigo.Focus();
            
            
        }

        

        private void Limpiar()
        {
            txtCodigo.Clear();
            txtCantidad.Text = "1";
            cmbMetodoPago.SelectedIndex = -1;
            txtDUI.Clear();
            txtObservacion.Clear();
            txtDineroCliente.Clear();
        }

        private void btnQuitar_Click(object sender, EventArgs e)
        {
            if (dgvVenta.CurrentRow == null || dgvVenta.CurrentRow.Index < 0)
            {
                MessageBox.Show("Por favor, selecciona un producto (haz clic en cualquier parte de la fila) para quitar.", "Advertencia",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow filaAQuitar = dgvVenta.CurrentRow;

            if (filaAQuitar.IsNewRow)
            {
                MessageBox.Show("No puedes quitar una fila vacía.");
                return;
            }

            var confirmar = MessageBox.Show("¿Estás seguro de que deseas quitar el producto seleccionado?", "Confirmar", MessageBoxButtons.YesNo);

            if (confirmar == DialogResult.Yes)
            {
                dgvVenta.Rows.RemoveAt(filaAQuitar.Index);
            }

            Limpiar();
            CalacularTotal();

        }

        private void CalacularTotal()
        {
            decimal total = 0;

            foreach (DataGridViewRow row in dgvVenta.Rows)
            {
                if (row.Cells["Subtotal"].Value != null)
                {
                    total += Convert.ToDecimal(row.Cells["Subtotal"].Value);
                }
            }

            lbTotal.Text = $"Total: ${total:0.00}";
        }

        
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDUI.Text))
            {
                MessageBox.Show("Por favor, ingrese el número de DUI a buscar.",
                                "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var uow = new UnitOfwork(_connectionString))
            {
                var cliente = uow.Cliente.GetByDUI(txtDUI.Text.Trim());

                if (cliente == null)
                {
                    MessageBox.Show("Cliente no encontrado. Se abrirá el formulario para agregarlo.",
                                    "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    using (var frmAgregarCliente = new frmCliente(_connectionString))
                    {
                        if (frmAgregarCliente.ShowDialog() == DialogResult.OK)
                        {

                            cliente = uow.Cliente.GetByDUI(txtDUI.Text.Trim());

                            if (cliente == null)
                            {
                                MessageBox.Show("No se pudo obtener el cliente recién agregado.",
                                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        else
                        {
                            // El usuario canceló el formulario sin guardar
                            return;
                        }
                    }
                }

       

                MessageBox.Show($"Cliente encontrado: {cliente.Nombres} {cliente.Apellidos}",
                                "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnVender_Click(object sender, EventArgs e)
        {
            var confirmar = MessageBox.Show("¿Desea completar la venta?", "Confirmar Venta", MessageBoxButtons.YesNo);
            if (string.IsNullOrWhiteSpace(txtDineroCliente.Text.Trim()))
            {
                MessageBox.Show("Por favor, ingrese el monto entregado por el cliente.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;

            }
            if (dgvVenta.Rows.Count == 0 || dgvVenta.Rows.Cast<DataGridViewRow>().All(r => r.IsNewRow))
            {
                MessageBox.Show("No hay productos en la venta.", "Advertencia",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (confirmar != DialogResult.Yes)
                return;

            Cliente cliente;
            using (var uow = new UnitOfwork(_connectionString))
            {
                cliente = uow.Cliente.GetByDUI(txtDUI.Text.Trim());
            }

            decimal total = decimal.Parse(lbTotal.Text.Replace("Total: $", ""));
            if (confirmar == DialogResult.Yes)
            {
                var venta = new Venta
                {
                    IdCliente = cliente.IdCliente,
                    FechaHora = DateTime.Now,
                    Total = total,
                    Subtotal = total,
                    Estado = true,
                    IdUsuario = IdUsuario, 
                    Observacion = txtObservacion.Text.Trim(),
                    IdMetodoPago = Convert.ToInt32(cmbMetodoPago.SelectedValue)
                };


                using (var uow = new UnitOfwork(_connectionString))
    {

                    var idGenerado = uow.Venta.Insert(venta);
                    venta.IdVenta = Convert.ToInt32(idGenerado);

                    foreach (DataGridViewRow row in dgvVenta.Rows)
                    {
                        if (row.IsNewRow) continue;


                        string codigoBarras = row.Cells["CodigoBarras"].Value.ToString();
                        int cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value);
                        decimal precioUnitario = Convert.ToDecimal(row.Cells["PrecioVenta"].Value);


                        var producto = uow.Producto.GetByCodigoBarras(codigoBarras);
                        if (producto == null)
                        {
                            MessageBox.Show($"Producto con código {codigoBarras} no encontrado.",
                                            "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            continue;
                        }


                        if (producto.Cantidad < cantidad)
                        {
                            MessageBox.Show($"No hay suficiente stock para {producto.Nombre}. " +
                                            $"Disponible: {producto.Cantidad}, solicitado: {cantidad}.",
                                            "Stock insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
;
                            continue;
                        }

                        decimal subtotalDetalle = precioUnitario * cantidad;


                        var detalleVenta = new DetalleVenta
                        {
                            IdVenta = venta.IdVenta,
                            IdProducto = producto.IdProducto,
                            Cantidad = cantidad,
                            PrecioUnitario = precioUnitario,
                            Subtotal = subtotalDetalle
                        };
                        uow.DetalleVenta.Insert(detalleVenta);


                        producto.Cantidad -= cantidad;

                        var camposActualizar = new List<Field>
                        {
                            new Field("Cantidad") 
                        };
                        uow.Producto.Update(producto, camposActualizar);
                    }

                    uow.Commit();
                }
                    decimal dineroEntregado = decimal.Parse(txtDineroCliente.Text.Trim());
                    decimal Total = decimal.Parse(lbTotal.Text.Replace("Total: $", ""));
                    decimal vuelto = dineroEntregado - Total;
                if (dineroEntregado < total)
                {
                    decimal faltante = total - dineroEntregado;
                    string mensaje = "¡Error de Venta! El pago es insuficiente." +
                                     $"\n\nTotal de la venta: {total:C2}" +
                                     $"\nDinero recibido: {dineroEntregado:C2}" +
                                     $"\nFaltan: {faltante:C2}";

                    MessageBox.Show(mensaje, "Pago Insuficiente",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; 
                }


                string mensajeExito = " ¡Venta completada con éxito!" +
                                      $"\n\nTotal de la venta: {total:C2}" +
                                      $"\nDinero recibido:   {dineroEntregado:C2}" +
                                      $"\nVUELTO a entregar: {vuelto:C2}";

                MessageBox.Show(mensajeExito, "Transacción Exitosa",
                MessageBoxButtons.OK, MessageBoxIcon.Information);


                dgvVenta.Rows.Clear();
                lbTotal.Text = "Total: $0.00";
                Limpiar();


            }
                
        }
    }
}
