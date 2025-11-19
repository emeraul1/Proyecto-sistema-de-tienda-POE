using RepoDb;
using Sistema_tienda_POE.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Sistema_tienda_POE.Repositorios
{
    public class DetalleVentaRepository
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;

        public DetalleVentaRepository(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public DetalleVenta GetById(int id)
        {
            return _connection.Query<DetalleVenta>(where: e => e.IdDetalleVenta == id, transaction: _transaction).FirstOrDefault();
        }

        public int Insert(DetalleVenta detalleVenta)
        {
            return (int)_connection.Insert(detalleVenta, transaction: _transaction);
        }

        public IEnumerable<DetalleVenta> GetAll()
        {
            return _connection.QueryAll<DetalleVenta>(transaction: _transaction);
        }

        public int Update(DetalleVenta detalleVenta, IEnumerable<Field> campos)
        {
            return _connection.Update<DetalleVenta>(detalleVenta, fields: campos, transaction: _transaction);
        }

     
    }
}
