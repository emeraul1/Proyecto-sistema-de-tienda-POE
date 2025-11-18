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
    public class VentaRepository
    {

        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;

        public VentaRepository(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public Venta GetById(int id)
        {
            return _connection.Query<Venta>(where: e => e.IdVenta == id, transaction: _transaction).FirstOrDefault();
        }

        public int Insert(Venta venta)
        {
            return (int)_connection.Insert(venta, transaction: _transaction);
        }

        public IEnumerable<Venta> GetAll()
        {
            return _connection.QueryAll<Venta>(transaction: _transaction);
        }

        public int Update(Venta venta, IEnumerable<Field> campos)
        {
            return _connection.Update<Venta>(venta, fields: campos, transaction: _transaction);
        }

        public IEnumerable<Venta> GetByEstado(bool estado)
        {
            return _connection.Query<Venta>(where: e => e.Estado == estado, transaction: _transaction);
        }
    }
}
