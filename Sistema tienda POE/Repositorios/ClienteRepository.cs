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
    public class ClienteRepository
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;

        public ClienteRepository(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public Cliente GetById(int id)
        {
            return _connection.Query<Cliente>(where: e => e.IdCliente == id, transaction: _transaction).FirstOrDefault();
        }

        public Cliente GetByDUI(string DUI)
        {
            return _connection.Query<Cliente>(where: e => e.DUI == DUI, transaction: _transaction).FirstOrDefault();
     
        }

        public int Insert(Cliente cliente)
        {
            return (int)_connection.Insert(cliente, transaction: _transaction);
        }

        public IEnumerable<Cliente> GetAll()
        {
            return _connection.QueryAll<Cliente>(transaction: _transaction);
        }

        public int Update(Cliente cliente, IEnumerable<Field> campos)
        {
            return _connection.Update<Cliente>(cliente, fields: campos, transaction: _transaction);
        }

        public IEnumerable<Cliente> GetByEstado(bool estado)
        {
            return _connection.Query<Cliente>(where: e => e.Estado == estado, transaction: _transaction);
        }
    }
}
