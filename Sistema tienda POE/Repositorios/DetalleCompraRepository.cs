using RepoDb;
using Sistema_tienda_POE.Clases;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace Sistema_tienda_POE.Repositorios
{
    public class DetalleCompraRepository
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;

        public DetalleCompraRepository(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public int Insert(DetalleCompra detalleCompra)
        {
            return (int)_connection.Insert(detalleCompra, transaction: _transaction);
        }

        public IEnumerable<DetalleCompra> GetByCompraId(int idCompra)
        {
            return _connection.Query<DetalleCompra>(where: e => e.IdCompra == idCompra, transaction: _transaction);
        }
    }
}