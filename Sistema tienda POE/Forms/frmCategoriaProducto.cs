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

namespace Sistema_tienda_POE.Forms
{
    public partial class frmCategoriaProducto : Form
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["Miconexion"].ConnectionString;
        public frmCategoriaProducto()
        {
            InitializeComponent();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNombreCategoria.Text))
            {
                MessageBox.Show("Por favor, completa todos los campos.");
                return;
            }

            var nuevaCategoria = new Clases.Categoria
            {
                Nombre = txtNombreCategoria.Text.Trim(),
                Descripcion = txtDescripcion.Text,
                Estado = true
            };

            using (var uow = new UnitOfwork(_connectionString))
            {
                uow.Categoria.Insert(nuevaCategoria);
                uow.Commit();
            }

            MessageBox.Show("Categoría agregada exitosamente.");
            txtNombreCategoria.Clear();
            txtDescripcion.Clear();

        }

        private void tbnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmCategoriaProducto_Load(object sender, EventArgs e)
        {

        }


    }
}
