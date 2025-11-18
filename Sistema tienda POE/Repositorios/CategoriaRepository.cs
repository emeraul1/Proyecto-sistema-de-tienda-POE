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
    public class CategoriaRepository
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;

        public CategoriaRepository(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public Categoria GetById(int id)
        {
            return _connection.Query<Categoria>(where: e => e.IdCategoria == id, transaction: _transaction).FirstOrDefault();
        }

        public int Insert(Categoria categoria)
        {
            return (int)_connection.Insert(categoria, transaction: _transaction);
        }

        public IEnumerable<Categoria> GetAll()
        {
            return _connection.QueryAll<Categoria>(transaction: _transaction);
        }

        public int Update(Categoria categoria, IEnumerable<Field> campos)
        {
            return _connection.Update<Categoria>(categoria, fields: campos, transaction: _transaction);
        }

        public IEnumerable<Categoria> GetByEstado(bool estado)
        {
            return _connection.Query<Categoria>(where: e => e.Estado == estado, transaction: _transaction);
        }
    }
}
