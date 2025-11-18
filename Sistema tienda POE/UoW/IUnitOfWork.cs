using Sistema_tienda_POE.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_tienda_POE.UoW
{
    public interface IUnitOfWork : IDisposable
    {
        UsuarioRepository Usuario { get; }
        RolResopitory Rol { get; }
        ProductoRepository Producto { get; }
        CategoriaRepository Categoria { get; }
        VentaRepository Venta { get; }
        MetodoPagoRepository MetodoPago { get; }
        ClienteRepository Cliente { get; }
        void Commit();
    }
}
