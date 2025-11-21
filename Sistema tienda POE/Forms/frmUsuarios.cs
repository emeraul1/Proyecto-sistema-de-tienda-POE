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
using RepoDb;
using Sistema_tienda_POE.Clases;

namespace Sistema_tienda_POE.Forms
{
    public partial class frmUsuarios : Form
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["conexionBD"].ConnectionString;
        public frmUsuarios()
        {
            InitializeComponent();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void frmUsuarios_Load(object sender, EventArgs e)
        {
            txtCodigo.CharacterCasing = CharacterCasing.Upper;
            CargarUsuarios();

            using (var ouw = new UnitOfwork(_connectionString))
            {
                var listaRoles = ouw.Rol.GetByEstado(true).ToList();
                cmbRol.DataSource = listaRoles;
                cmbRol.DisplayMember = "Nombre";
                cmbRol.ValueMember = "IdRol";
                cmbRol.SelectedIndex = -1;
            }
        }

        private void CargarUsuarios()
        {
            using (var ouw = new UnitOfwork(_connectionString))
            {
                var lista = ouw.Usuario.GetByEstado(true).ToList();
                var roles = ouw.Rol.GetAll().ToList();
                foreach (var u in lista)
                {
                    var rol = roles.FirstOrDefault(r => r.IdRol == u.IdRol);
                    u.NombreRol = rol != null ? rol.Nombre : "Sin rol";
                }
                dgvUsurios.DataSource = lista;
                dgvUsurios.Columns["IdUsuario"].Visible = false;
                dgvUsurios.Columns["Contraseña"].Visible = false;
                dgvUsurios.Columns["Estado"].Visible = true;
                dgvUsurios.Columns["IdRol"].Visible = false;
                dgvUsurios.Columns["NombreRol"].HeaderText = "Rol";
                dgvUsurios.Refresh();

            }
        }
        private int usuarioSeleccionadoId = 0;
        private void LimpiarControles()
        {
            txtApellidos.Clear();
            txtNombres.Clear();
            txtCodigo.Clear();
            txtContraseña.Clear();
            cmbRol.SelectedIndex = -1;
            usuarioSeleccionadoId = 0;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if(usuarioSeleccionadoId > 0)
            {
                MessageBox.Show("Este usuario solo se puede Actualizar");
                LimpiarControles();
                return;
            }

            if(string.IsNullOrWhiteSpace(txtNombres.Text) ||
               string.IsNullOrWhiteSpace(txtApellidos.Text) ||
               string.IsNullOrWhiteSpace(txtCodigo.Text) ||
               string.IsNullOrWhiteSpace(txtContraseña.Text) ||
               cmbRol.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor complete todos los campos.");
                return;
            }

            var clave = txtContraseña.Text.Trim();
            var claveEncriptada = Encriptador.EncriptarClave(clave);

            var usuario = new Clases.Usuario
            {
                Nombres = txtNombres.Text.Trim(),
                Apellidos = txtApellidos.Text.Trim(),
                NombreUsuario = txtCodigo.Text.Trim().ToUpper(),
                Contraseña = claveEncriptada,
                IdRol = (int)cmbRol.SelectedValue,
                Estado = true,
                FechaRegistro = DateTime.Now
            };

            using (var ouw = new UnitOfwork(_connectionString))
            {
                ouw.Usuario.Insert(usuario);
                ouw.Commit();
            }

            MessageBox.Show("Usuario agregado correctamente.");
            LimpiarControles();
            CargarUsuarios();
        }

        private void btnRemover_Click(object sender, EventArgs e)
        {
            if (usuarioSeleccionadoId == 0)
            {
                MessageBox.Show("Seleccione un usuario para eliminar.");
                return;
            }

            var confirmar = MessageBox.Show("Esta seguro que desea eliminar este usuario?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmar != DialogResult.Yes) return; //usrio cqncelo la eliminacion

            var usuario = new Clases.Usuario
            {
                IdUsuario = usuarioSeleccionadoId,
                Estado = false
            };

            var camposActualizar = new List<Field>();
            camposActualizar.Add(new Field("Estado" ));

            using (var ouw = new UnitOfwork(_connectionString))
            {
                if (camposActualizar.Any())
                {
                    ouw.Usuario.Update(usuario, camposActualizar.ToArray());
                    ouw.Commit();
                    MessageBox.Show("Usuario eliminado correctamente");
                    LimpiarControles();
                    usuarioSeleccionadoId = 0;
                    CargarUsuarios();

                } else 
                {
                    MessageBox.Show("No se seleccionaron campos para actualizar.");
                }
                
            }
            
               
        }

        private void dgvUsurios_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var usuario = (Usuario)dgvUsurios.Rows[e.RowIndex].DataBoundItem;

                using (var formEditar = new frmUsuarioCmd(usuario.IdUsuario, _connectionString))
                {
                    var rersult = formEditar.ShowDialog();
                    if (rersult == DialogResult.OK)
                    {
                        CargarUsuarios();
                    }
                }
            }
        }

        private void dgvUsurios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0)
            {
                var usuario = (Usuario)dgvUsurios.Rows[e.RowIndex].DataBoundItem;
                usuarioSeleccionadoId = usuario.IdUsuario;
                txtNombres.Text = usuario.Nombres;
                txtApellidos.Text = usuario.Apellidos;
                txtCodigo.Text = usuario.NombreUsuario;
                cmbRol.SelectedValue = usuario.IdRol;
                txtContraseña.Clear();
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtApellidos.Clear();
            txtNombres.Clear();
            txtCodigo.Clear();
            txtContraseña.Clear();
            cmbRol.SelectedIndex = -1;
        }
    }
}
