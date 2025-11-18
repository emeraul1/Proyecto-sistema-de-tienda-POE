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
    public class MetodoPagoRepository
    {

        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;

        public MetodoPagoRepository(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public MetodoPago GetById(int id)
        {
            return _connection.Query<MetodoPago>(where: e => e.IdMetodoPago == id, transaction: _transaction).FirstOrDefault();
        }

        public int Insert(MetodoPago metodoPago)
        {
            return (int)_connection.Insert(metodoPago, transaction: _transaction);
        }

        public IEnumerable<MetodoPago> GetAll()
        {
            return _connection.QueryAll<MetodoPago>(transaction: _transaction);
        }

        public int Update(MetodoPago metodoPago, IEnumerable<Field> campos)
        {
            return _connection.Update<MetodoPago>(metodoPago, fields: campos, transaction: _transaction);
        }

        public IEnumerable<MetodoPago> GetByEstado(bool estado)
        {
            return _connection.Query<MetodoPago>(where: e => e.Estado == estado, transaction: _transaction);
        }
    }
}
