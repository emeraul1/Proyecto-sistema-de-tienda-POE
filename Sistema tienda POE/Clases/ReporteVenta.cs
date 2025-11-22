using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_tienda_POE.Clases
{
    public class ReporteVenta
    {
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public string Categoria { get; set; }
        public decimal PrecioUnitario { get; set; }
        public DateTime Fecha { get; set; }
        public string VendidoPor { get; set; }
        public string MetodoPago { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal Total { get; set; }
    }
}
