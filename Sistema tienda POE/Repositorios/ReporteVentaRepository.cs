using Sistema_tienda_POE.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Data.SqlClient;
using RepoDb;

namespace Sistema_tienda_POE.Repositorios
{
    public class ReporteVentaRepository
    {

        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;

        public ReporteVentaRepository(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public List<ReporteVenta> ObtenerReporteVentas(DateTime inicio, DateTime fin, int idCategoria)
        {
            string sql = @"
        SELECT 
            p.Nombre AS Producto,
            dv.Cantidad AS Cantidad,
            c.Nombre AS Categoria,
            dv.PrecioUnitario AS PrecioUnitario,
            v.FechaHora AS Fecha, 
             LTRIM(RTRIM(u.Nombres + ' ' + ISNULL(u.Apellidos, ''))) AS VendidoPor,
            mp.Nombre AS MetodoPago,
            p.Costo AS CostoUnitario,
            (dv.Cantidad * dv.PrecioUnitario) AS Total
        FROM DetalleVenta dv
        INNER JOIN Venta v ON dv.IdVenta = v.IdVenta
        INNER JOIN Producto p ON dv.IdProducto = p.IdProducto
        JOIN Categoria c ON p.IdCategoria = c.IdCategoria
        INNER JOIN Usuario u ON v.IdUsuario = u.IdUsuario
        INNER JOIN MetodoPago mp ON v.IdMetodoPago = mp.IdMetodoPago
        WHERE v.FechaHora >= @Inicio
            AND v.FechaHora < DATEADD(DAY, 1, @Fin)
            AND (@IdCategoria = 0 OR c.IdCategoria = @IdCategoria)
        ORDER BY v.FechaHora DESC;
    ";

            return _connection.ExecuteQuery<ReporteVenta>(
                sql,
                new { Inicio = inicio, Fin = fin, IdCategoria = idCategoria },
                transaction: _transaction
            ).ToList();
        }
    }
}
