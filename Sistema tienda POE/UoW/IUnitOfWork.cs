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
        void Commit();
    }
}
