using RepoDb;
using Sistema_tienda_POE.Clases;
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

namespace Sistema_tienda_POE.Forms
{
    public partial class frmProductosCmd : Form
    {
        private readonly string _connectionString;
        private readonly int _productoId;
        public frmProductosCmd(int productoId, string ConnectionString)
        {
            InitializeComponent();
            _connectionString = ConnectionString;
            _productoId = productoId;
        }

        private void frmProductosCmd_Load(object sender, EventArgs e)
        {
            chbActivo.Checked = true;

            using (var ouw = new UoW.UnitOfwork(_connectionString))
            {
                var producto = ouw.Producto.GetById(_productoId);
               
                using (var uow = new UnitOfwork(_connectionString))
                {
                    var listaRoles = ouw.Categoria.GetByEstado(true).ToList();
                    cmbCategoria.DataSource = listaRoles;
                    cmbCategoria.DisplayMember = "Nombre";
                    cmbCategoria.ValueMember = "IdCategoria";
                    cmbCategoria.SelectedIndex = -1;

                    producto = ouw.Producto.GetById(_productoId);
                }
                if (producto == null)
                {
                    MessageBox.Show("Producto no encontrado.");
                    this.Close();
                    return;
                }
                txtNombre.Text = producto.Nombre;
                txtCodigo.Text = producto.CodigoBarras;
                txtMarca.Text = producto.Marca;
                txtPrecioVenta.Text = producto.PrecioVenta.ToString("F2");
                txtModelo.Text = producto.Modelo.ToString();
                txtStockMinimo.Text = producto.StockMinimo.ToString();
                txtCosto.Text = producto.Costo.ToString("F2");
                txtCantidad.Text = producto.Cantidad.ToString();
                chbActivo.Checked = producto.Estado;

                if (producto.IdCategoria > 0)
                {
                    cmbCategoria.SelectedValue = producto.IdCategoria;
                }

            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
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

            var productoActualizado = new Producto
            {
                IdProducto = _productoId,
                Nombre = txtNombre.Text.Trim(),
                Cantidad = int.Parse(txtCantidad.Text.Trim()),
                CodigoBarras = txtCodigo.Text.Trim(),
                Costo = decimal.Parse(txtCosto.Text.Trim()),
                PrecioVenta = decimal.Parse(txtPrecioVenta.Text.Trim()),
                IdCategoria = ((Categoria)cmbCategoria.SelectedItem).IdCategoria,
                Marca = txtMarca.Text.Trim(),
                Modelo = txtModelo.Text.Trim(),
                StockMinimo = int.Parse(txtStockMinimo.Text.Trim()),
                Estado = chbActivo.Checked
            };

            var camposActualizar = new List<Field>
            {
                new Field("Nombre"),
                new Field("Cantidad"),
                new Field("CodigoBarras"),
                new Field("Costo"),
                new Field("PrecioVenta"),
                new Field("IdCategoria"),
                new Field("Marca"),
                new Field("Modelo"),
                new Field("StockMinimo"),
                new Field("Estado")
            };

            using (var ouw = new UnitOfwork(_connectionString))
            {
                ouw.Producto.Update(productoActualizado, camposActualizar);
                ouw.Commit();
                MessageBox.Show("Producto actualizado correctamente.");
                this.DialogResult = DialogResult.OK;
                this.Close();

            }
        }

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

        private void SoloLetras(object sender, KeyPressEventArgs e)
        {
            //Validar si solo es letra 
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Solo se permiten letras", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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


    }
}
