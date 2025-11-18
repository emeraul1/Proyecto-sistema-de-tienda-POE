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
    public class RolResopitory
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;

        public RolResopitory(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public Rol GetById(int id)
        {
            return _connection.Query<Rol>(where: e => e.IdRol == id, transaction: _transaction).FirstOrDefault();
        }

        public int Insert(Rol rol)
        {
            return (int)_connection.Insert(rol, transaction: _transaction);
        }

        public IEnumerable<Rol> GetAll()
        {
            return _connection.QueryAll<Rol>(transaction: _transaction);
        }

        public int Update(Rol rol, IEnumerable<Field> campos)
        {
            return _connection.Update<Rol>(rol, fields: campos, transaction: _transaction);
        }

        public IEnumerable<Rol> GetByEstado(bool estado)
        {
            return _connection.Query<Rol>(where: e => e.Estado == estado, transaction: _transaction);
        }

    }
}
