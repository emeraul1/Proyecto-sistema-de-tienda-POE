using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_tienda_POE.Clases
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int StockMinimo { get; set; }
        public string CodigoBarras { get; set; }
        public int IdCategoria { get; set; }
        public decimal Costo { get; set; }
        public decimal PrecioVenta { get; set; }
        public int Cantidad { get; set; }

        public bool Estado { get; set; }

        public string CategoriaNombre { get; set; }

    }
}
