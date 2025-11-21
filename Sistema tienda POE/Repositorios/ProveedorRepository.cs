using RepoDb;
using Sistema_tienda_POE.Clases;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using System;

namespace Sistema_tienda_POE.Repositorios
{
    public class ProveedorRepository
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;

        // Constructor con transacción opcional
        public ProveedorRepository(SqlConnection connection, SqlTransaction transaction = null)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public int Insert(Proveedor proveedor)
        {
            return (int)_connection.Insert(proveedor, transaction: _transaction);
        }
        public Proveedor GetById(int id)
        {
            return _connection.Query<Proveedor>(where: e => e.IdProveedor == id, transaction: _transaction).FirstOrDefault();
        }

        public IEnumerable<Proveedor> GetByEstado(bool estado)
        {
            return _connection.Query<Proveedor>(where: e => e.Estado == estado, transaction: _transaction);
        }

        // Actualiza el objeto completo
        public int Update(Proveedor proveedor)
        {
            return _connection.Update(proveedor, transaction: _transaction);
        }

        public int EliminarLogico(int idProveedor)
        {
            //  Definimos la sentencia SQL simple para cambiar el estado
            string sql = "UPDATE Proveedor SET Estado = @Estado WHERE IdProveedor = @Id";

            // Creamos un objeto anónimo para los parámetros.
            var parametros = new { Estado = false, Id = idProveedor };

            // aqui ejecutamos la consulta SQL directamente para evitar la sobrecarga conflictiva de Update
            return _connection.ExecuteNonQuery(
                sql,
                param: parametros,
                transaction: _transaction
            );
        }
    }
}