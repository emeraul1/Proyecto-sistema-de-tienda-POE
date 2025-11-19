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
        private RolResopitory _rolRepo;
        private ProductoRepository _productoRepo;
        private CategoriaRepository _categoriaRepo;
        private VentaRepository _ventaRepo;
        private ClienteRepository _clienteRepo;
        private MetodoPagoRepository _metodoPagoRepo;
        private DetalleVentaRepository _detalleVentaRepo;

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

        public RolResopitory Rol
        {
            get
            {
                if (_rolRepo == null)
                {
                    _rolRepo = new RolResopitory(_connection, _transaction);
                }
                return _rolRepo;

            }
        }

        public ProductoRepository Producto
        {
            get
            {
                if (_productoRepo == null)
                {
                    _productoRepo = new ProductoRepository(_connection, _transaction);
                }
                return _productoRepo;
            }
        }

        public CategoriaRepository Categoria
        {
            get
            {
                if (_categoriaRepo == null)
                {
                    _categoriaRepo = new CategoriaRepository(_connection, _transaction);
                }
                return _categoriaRepo;
            }
        }

        public VentaRepository Venta
        {
            get
            {
                if (_ventaRepo == null)
                {
                    _ventaRepo = new VentaRepository(_connection, _transaction);
                }
                return _ventaRepo;
            }
        }

        public ClienteRepository Cliente
        {
            get
            {
                if (_clienteRepo == null)
                {
                    _clienteRepo = new ClienteRepository(_connection, _transaction);
                }
                return _clienteRepo;
            }
        }

        public MetodoPagoRepository MetodoPago
        {
            get
            {
                if (_metodoPagoRepo == null)
                {
                    _metodoPagoRepo = new MetodoPagoRepository(_connection, _transaction);
                }
                return _metodoPagoRepo;
            }
        }

        public DetalleVentaRepository DetalleVenta
        {
            get
            {
                if (_detalleVentaRepo == null)
                {
                    _detalleVentaRepo = new DetalleVentaRepository(_connection, _transaction);
                }
                return _detalleVentaRepo;
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
