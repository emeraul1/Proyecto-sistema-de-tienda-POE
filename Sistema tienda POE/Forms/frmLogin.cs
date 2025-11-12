using Microsoft.IdentityModel.Protocols;
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

namespace Sistema_tienda_POE
{
    public partial class frmLogin : Form
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["Miconexion"].ConnectionString;
        public frmLogin()
        {
            InitializeComponent();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            txtContraseña.UseSystemPasswordChar = true; //activar la mascara en la contrseña 
            txtContraseña.CharacterCasing = CharacterCasing.Lower; //convertir a miniculas
          //  txtNombreUsuario.CharacterCasing = CharacterCasing.Upper; //convertir a mayusculas
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreUsuario.Text) || string.IsNullOrWhiteSpace(txtContraseña.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos, usuario y contraseña.");
            }

            //Buscar eb bd

            using (var uow = new UoW.UnitOfwork(_connectionString))
            {
                string codigo = txtNombreUsuario.Text.Trim();
                var clave = txtContraseña.Text.Trim();
                var claveEncriptada = Encriptador.EncriptarClave(clave);

                var usuario = uow.Usuario.ObtenerPorCredenciales(codigo, claveEncriptada);

                if (usuario != null)
                {
                    if(usuario.IdRol == 1)
                    {
                        //usuario encontrado
                        MessageBox.Show($"Bienvenido {usuario.Nombres} {usuario.Apellidos} Rol: Administrador.", "Acceso permitido.",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.DialogResult = DialogResult.OK;                   
                    }
                    if (usuario.IdRol == 2)
                    {
                        //usuario encontrado
                        MessageBox.Show($"Bienvenido {usuario.Nombres} {usuario.Apellidos} Rol: Cajero.", "Acceso permitido.",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.DialogResult = DialogResult.Yes;
                    }
                }
                else
                {
                    MessageBox.Show("Credenciales incorrectas. Intente nuevamente.", "Error de acceso",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
