using RepoDb;
using Sistema_tienda_POE.Clases;
using Microsoft.Data.SqlClient;
using System;

namespace Sistema_tienda_POE.Repositorios
{
    /// <summary>
    /// Repositorio para gestionar las transacciones de Compra.
    /// Requiere una conexión y una transacción activas.
    /// </summary>
    public class CompraRepository
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;

        // Constructor que recibe la conexión y la transacción.
        public CompraRepository(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        /// <summary>
        /// Inserta el registro de la Compra (encabezado) y devuelve el Id generado.
        /// </summary>
        public int InsertCompra(Compra compra)
        {
            // RepoDB inserta y devuelve el valor de la clave primaria (IdCompra).
            // La transacción garantiza la atomicidad.
            return (int)_connection.Insert(compra, transaction: _transaction);
        }

        /// <summary>
        /// Inserta un registro de detalle de la Compra.
        /// </summary>
        public int InsertDetalle(DetalleCompra detalle)
        {
            // Insertamos el detalle dentro de la misma transacción.
            return (int)_connection.Insert(detalle, transaction: _transaction);
        }

        /// <summary>
        /// Aumenta la cantidad (Stock) de un producto.
        /// </summary>
        public int ActualizarStock(int idProducto, int cantidadComprada)
        {

            // Se asume que en tu tabla Producto, la columna de Stock se llama 'Stock'
            // y que en CompraRepository no tienes acceso directo al costo para el cálculo.

            return _connection.ExecuteNonQuery(
                "UPDATE Producto SET Cantidad = Cantidad + @Cantidad WHERE IdProducto = @IdProducto",
                param: new { Cantidad = cantidadComprada, IdProducto = idProducto },
                transaction: _transaction
            );
        }
    }
}