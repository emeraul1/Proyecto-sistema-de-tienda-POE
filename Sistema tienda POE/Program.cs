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
            SqlServerBootstrap.Initialize();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            frmLogin login = new frmLogin();

            if (login.ShowDialog() == DialogResult.OK)
            {

                Application.Run(new frmPrincipalAdministrador());
            }

            if (login.DialogResult == DialogResult.Yes)
            {
                Application.Run(new frmPrincipalCajero());
            }
        }
    }
}
