using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepoDb;
using Microsoft.Data.SqlClient;
using Sistema_tienda_POE;
using Sistema_tienda_POE.Repositorios;

namespace Sistema_tienda_POE.UoW
{
    public class UnitOfwork : IUnitOfWork
    {
        private readonly SqlConnection _connection;
        private SqlTransaction _transaction;
        private UsuarioRepository _usuarioRepo;

        public UnitOfwork(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public UsuarioRepository Usuario
        {
            get
            {
                if (_usuarioRepo == null)
                {
                    _usuarioRepo = new UsuarioRepository(_connection, _transaction);
                }
                return _usuarioRepo;
            }
        }
        public void Commit()
        {
            _transaction?.Commit();
            _transaction?.Dispose();
            _transaction = null;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }
}
