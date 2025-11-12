using RepoDb;
using Sistema_tienda_POE.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

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

        public Usuario GetById(int id)
        {
            return _connection.Query<Usuario>(where: e => e.IdUsuario == id, transaction: _transaction).FirstOrDefault();
        }

        public int Insert(Usuario rol)
        {
            return (int)_connection.Insert(rol, transaction: _transaction);
        }

        public IEnumerable<Usuario> GetAll()
        {
            return _connection.QueryAll<Usuario>(transaction: _transaction);
        }

        public int Update(Usuario rol, IEnumerable<Field> campos)
        {
            return _connection.Update<Usuario>(rol, fields: campos, transaction: _transaction);
        }

        public IEnumerable<Usuario> GetByEstado(bool estado)
        {
            return _connection.Query<Usuario>(where: e => e.Estado == estado, transaction: _transaction);
        }

    }
}
