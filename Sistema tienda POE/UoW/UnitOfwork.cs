using Microsoft.Data.SqlClient;
using Sistema_tienda_POE.Clases;
using Sistema_tienda_POE.Repositorios;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;



namespace Sistema_tienda_POE.UoW
{
    public class UnitOfwork : IDisposable
    {
        private readonly SqlConnection _connection;
        private SqlTransaction _transaction;

        public UsuarioRepository Usuario { get; private set; }
        public VentaRepository Venta { get; private set; }
        public DetalleVentaRepository DetalleVenta { get; private set; }
        public ProductoRepository Producto { get; private set; }
        public CategoriaRepository Categoria { get; private set; }
        public ClienteRepository Cliente { get; private set; }
        public MetodoPagoRepository MetodoPago { get; private set; }
        public RolResopitory Rol { get; private set; }

        public ProveedorRepository Proveedor { get; private set; }
        public CompraRepository Compra { get; private set; }
        public DetalleCompraRepository DetalleCompra { get; private set; }

        public ReporteVentaRepository ReporteVenta { get; private set; }

        public UnitOfwork(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();

            Usuario = new UsuarioRepository(_connection, _transaction);
            Venta = new VentaRepository(_connection, _transaction);
            DetalleVenta = new DetalleVentaRepository(_connection, _transaction);
            Producto = new ProductoRepository(_connection, _transaction);
            Categoria = new CategoriaRepository(_connection, _transaction);
            Cliente = new ClienteRepository(_connection, _transaction);
            MetodoPago = new MetodoPagoRepository(_connection, _transaction);
            Rol = new RolResopitory(_connection, _transaction);
            ReporteVenta = new ReporteVentaRepository (_connection, _transaction);

            Proveedor = new ProveedorRepository(_connection, _transaction);
            Compra = new CompraRepository(_connection, _transaction);
            DetalleCompra = new DetalleCompraRepository(_connection, _transaction);
        }

        public void Commit()
        {
            try
            {
                _transaction.Commit();
                _transaction = _connection.BeginTransaction(); // inicia una nueva transaccion para futuras operaciones
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
        }

        public void Rollback()
        {
            _transaction.Rollback();
            _transaction = _connection.BeginTransaction();
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
            }
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}