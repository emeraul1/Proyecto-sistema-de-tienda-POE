using Sistema_tienda_POE.Clases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sistema_tienda_POE.UoW;
using System.Configuration;
using RepoDb;
using Microsoft.Data.SqlClient;

namespace Sistema_tienda_POE.Forms
{

    public partial class frmUsuarioCmd : Form
    {
        private readonly string _connectionString;
        private readonly int _usuarioId;
        public frmUsuarioCmd(int usuarioId, string connectionString)
        {
            InitializeComponent();
            _usuarioId = usuarioId;
            _connectionString = connectionString;
        }

        private void frmUsuarioCmd_Load(object sender, EventArgs e)
        {
            txtCodigo.CharacterCasing = CharacterCasing.Upper;
            chbEstado.Checked = true;

            Usuario usuario = null;

            using (var ouw = new UnitOfwork(_connectionString))
            {
                var listaRoles = ouw.Rol.GetByEstado(true).ToList();
                cmbRol.DataSource = listaRoles;
                cmbRol.DisplayMember = "Nombre";
                cmbRol.ValueMember = "IdRol";
                cmbRol.SelectedIndex = -1;

                usuario = ouw.Usuario.GetById(_usuarioId);
            }

            if (usuario == null)
             {
                MessageBox.Show("Usuario no encontrado.");
                this.Close();
                return;
            }

            txtNombres.Text = usuario.Nombres;
            txtApellidos.Text = usuario.Apellidos;
            txtCodigo.Text = usuario.NombreUsuario;
            txtContraseña.Text = ""; // Nunca mostrar la clave en el campo
            chbEstado.Checked = usuario.Estado;

            if (usuario.IdRol > 0)
            {
                cmbRol.SelectedValue = usuario.IdRol;
            }

        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombres.Text) ||
                string.IsNullOrWhiteSpace(txtApellidos.Text) ||
                string.IsNullOrWhiteSpace(txtCodigo.Text) ||
                string.IsNullOrWhiteSpace(txtContraseña.Text) ||
                cmbRol.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return;
            }

            var clave = txtContraseña.Text.Trim();
            var claveEncriptada = Encriptador.EncriptarClave(clave);

            var usuario = new Usuario
            {
                IdUsuario = _usuarioId,
                Nombres = txtNombres.Text.Trim(),
                Apellidos = txtApellidos.Text.Trim(),
                NombreUsuario = txtCodigo.Text.Trim().ToUpper(),
                Contraseña = claveEncriptada,
                IdRol = (int)cmbRol.SelectedValue,
                Estado = chbEstado.Checked
            };

            var camposActualizar = new List<Field>
            {
                new Field("Nombres"),
                new Field("Apellidos"),
                new Field("NombreUsuario"),
                new Field("Contraseña"),
                new Field("IdRol"),
                new Field("Estado")
            };

            using (var uow = new UnitOfwork(_connectionString))
            {
                try
                {
                    uow.Usuario.Update(usuario, camposActualizar);
                    uow.Commit();
                    MessageBox.Show("Usuario actualizado correctamente.");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al actualizar el usuario: {ex.Message}");
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
