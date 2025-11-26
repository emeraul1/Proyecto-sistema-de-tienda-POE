using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using RepoDb;
using Microsoft.Data.SqlClient;
using Sistema_tienda_POE.Forms;

namespace Sistema_tienda_POE
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SqlServerBootstrap.Initialize(); // habilitador de RepoDB para SQL Server 

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            frmLogin login = new frmLogin();
            //frmUsuarios login = new frmUsuarios();


            // Mostrar el formulario de login como un cuadro de diálogo modal
            DialogResult result = login.ShowDialog();
            GlobalConfiguration.Setup().UseSqlServer();

            if (result == DialogResult.OK)
            {
                Application.Run(new frmPrincipalAdministrador());

            }
            else if (result == DialogResult.Yes)
            {
                Application.Run(new frmPrincipalCajero());
            }
        }
    }
}
