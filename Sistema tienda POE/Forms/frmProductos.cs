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
    public partial class frmProductos : Form
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["Miconexion"].ConnectionString;
        public frmProductos()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void frmProductos_Load(object sender, EventArgs e)
        {
            CargarProductos();
            CargarCategorias();
        }

        private void CargarProductos()
        {
            using (var uow = new UnitOfwork(_connectionString))
            {
                var productos = uow.Producto.GetByEstado(true).ToList();
                var categorias = uow.Categoria.GetAll().ToList();
                foreach (var p in productos)
                {
                    var cat = categorias.FirstOrDefault(c => c.IdCategoria == p.IdCategoria);
                    p.CategoriaNombre = cat != null ? cat.Nombre : "Sin categoría";
                }
                
                dgvProductos.DataSource = productos;
                dgvProductos.Columns["IdProducto"].Visible = false;
                dgvProductos.Columns["Estado"].Visible = false;
                dgvProductos.Columns["IdCategoria"].Visible = false;
                dgvProductos.Columns["CategoriaNombre"].HeaderText = "Categoria";
                dgvProductos.Refresh();


            }

           
        }

        private void CargarCategorias()
        {
            using (var uow = new UnitOfwork(_connectionString))
            {
                var listaCategorias = uow.Categoria.GetByEstado(true).ToList();
                listaCategorias.Add(new Categoria
                {
                    IdCategoria = 0,
                    Nombre = "Agregar nueva categoría..."
                });
                cmbCategoria.DataSource = null;
                cmbCategoria.DataSource = listaCategorias;
                cmbCategoria.DisplayMember = "Nombre";
                cmbCategoria.ValueMember = "IdCategoria";
                cmbCategoria.SelectedIndex = -1;
                cmbCategoria.Refresh();
            }
        }

        private void cmbCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCategoria.SelectedItem is Categoria cat && cat.IdCategoria == 0)
            {
                using (var frm = new frmCategoriaProducto())
                {
                    var result = frm.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        CargarCategorias();
                    }

                    this.cmbCategoria.SelectedIndex = -1;
                    
                }
            }
        }

        private void LimpiarControles()
        {
            txtNombre.Clear();
            txtCantidad.Clear();
            txtCodigo.Clear();
            txtCosto.Clear();
            txtPrecioVenta.Clear();
            cmbCategoria.SelectedIndex = -1;
            txtMarca.Clear();
            txtModelo.Clear();

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txtNombre.Text) ||
               string.IsNullOrWhiteSpace(txtCantidad.Text) ||
               string.IsNullOrWhiteSpace(txtCodigo.Text) ||
               string.IsNullOrWhiteSpace(txtCosto.Text) ||
               string.IsNullOrWhiteSpace(txtPrecioVenta.Text) ||
               cmbCategoria.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return;
            }

            var nuevoProducto = new Producto
            {
                Nombre = txtNombre.Text.Trim(),
                Cantidad = int.Parse(txtCantidad.Text.Trim()),
                CodigoBarras = txtCodigo.Text.Trim(),
                Costo = decimal.Parse(txtCosto.Text.Trim()),
                PrecioVenta =  decimal.Parse(txtPrecioVenta.Text.Trim()),
                IdCategoria = ((Categoria)cmbCategoria.SelectedItem).IdCategoria,
                Marca = txtMarca.Text.Trim(),
                StockMinimo = int.Parse(txtStockMinimo.Text.Trim()),
                Modelo = txtModelo.Text.Trim(),
                Estado = true
            };

            using (var uow = new UnitOfwork(_connectionString))
            {
                uow.Producto.Insert(nuevoProducto);
                uow.Commit();
            }
            MessageBox.Show("Producto agregado correctamente.");
            LimpiarControles();
            CargarProductos();
        }

        private int productoSeleccionadoId = 0;
        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0)
            {
                var producto = (Producto)dgvProductos.Rows[e.RowIndex].DataBoundItem;
                productoSeleccionadoId = producto.IdProducto;
                txtCantidad.Text = producto.Cantidad.ToString();
                txtCodigo.Text = producto.CodigoBarras;
                txtCosto.Text = producto.Costo.ToString("F2");
                txtMarca.Text = producto.Marca; 
                txtModelo.Text = producto.Modelo;
                txtNombre.Text = producto.Nombre;
                txtPrecioVenta.Text = producto.PrecioVenta.ToString("F2");
                txtStockMinimo.Text = producto.StockMinimo.ToString();
                if(producto.IdCategoria > 0)
                {
                    cmbCategoria.SelectedValue = producto.IdCategoria;
                }
                else
                {
                    cmbCategoria.SelectedIndex = -1;
                }
            }
        }

        private void dgvProductos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0)
            {
                var producto = (Producto)dgvProductos.Rows[e.RowIndex].DataBoundItem;

                using (var frmeditar = new frmProductosCmd(producto.IdProducto, _connectionString))
                {
                    var result = frmeditar.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        CargarProductos();
                    }
                }
            }
        }

        //metodos para validar datos 

        private void SoloNumeros(object sender, KeyPressEventArgs e)
        {
            //Validar que solo sea número
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Bloquea cualquier otra tecla
                MessageBox.Show("Solo se permiten números", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SoloDecimales(object sender, KeyPressEventArgs e)
        {
            // Permitir solo dígitos y un punto decimal
            TextBox txt = sender as TextBox;
            if (char.IsControl(e.KeyChar))
                return;
            if (char.IsDigit(e.KeyChar))
                return;

            // Permitir un solo punto decimal
            if (e.KeyChar == '.' && !txt.Text.Contains("."))
                return;

            // Bloquear todo lo demás
            e.Handled = true;
            MessageBox.Show("Solo se permiten números y un punto decimal", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void txtCosto_KeyPress(object sender, KeyPressEventArgs e)
        {
            SoloDecimales(sender, e);
        }

        private void txtPrecioVenta_KeyPress(object sender, KeyPressEventArgs e)
        {
            SoloDecimales(sender, e);
        }

        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            SoloNumeros(sender, e);
        }

        private void txtStockMinimo_KeyPress(object sender, KeyPressEventArgs e)
        {
            SoloNumeros(sender, e);
        }

        //formato data griv si es menor el stock minimo a la cantidad 
        private void dgvProductos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvProductos.Columns[e.ColumnIndex].Name == "Cantidad")
            {
                int cantidad = Convert.ToInt32(dgvProductos.Rows[e.RowIndex].Cells["Cantidad"].Value);
                int stockMinimo = Convert.ToInt32(dgvProductos.Rows[e.RowIndex].Cells["StockMinimo"].Value);

                if (cantidad <= stockMinimo)
                {
                    dgvProductos.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                }
                else
                {
                    dgvProductos.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                }
            }
        }

        private void btnRemover_Click(object sender, EventArgs e)
        {
            if (productoSeleccionadoId == 0)
            {
                MessageBox.Show("Seleccione un producto para eliminar.");
                return;
            }
            var confirmar = MessageBox.Show("¿Está seguro de que desea eliminar este producto?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmar != DialogResult.Yes)
            {
                LimpiarControles();
                productoSeleccionadoId  = 0;
                return;  // El usuario canceló la eliminación
            }
                

            var producto = new Producto
            {
                IdProducto = productoSeleccionadoId,
                Estado = false
            };

            var camposActualizar = new List<Field>
            {
                new Field("Estado")
            };

            using (var uow = new UnitOfwork(_connectionString))
            {
                if (camposActualizar.Any())
                {
                    uow.Producto.Update(producto, camposActualizar);
                    uow.Commit();
                    MessageBox.Show("Producto eliminado correctamente.");
                    CargarProductos();
                    LimpiarControles();

                }
                else
                {
                    MessageBox.Show("No hay campos para actualizar.");
                }

            }
        }
    }
}
