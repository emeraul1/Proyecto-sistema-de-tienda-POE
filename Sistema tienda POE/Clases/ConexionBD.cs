using Microsoft.Data.SqlClient;
using System.Configuration;

namespace Sistema_tienda_POE.Clases
{
    public static class ConexionBD
    {
        private static string _connectionString = ConfigurationManager.ConnectionStrings["conexionBD"].ConnectionString;

        public static SqlConnection ObtenerConexion()
        {
            // Devuelve una nueva instancia de SqlConnection cada vez que se llama
            return new SqlConnection(_connectionString);
        }
    }
}