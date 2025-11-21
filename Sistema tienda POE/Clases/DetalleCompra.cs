using System;

namespace Sistema_tienda_POE.Clases
{
    public class DetalleCompra
    {
        public int IdDetalleCompra { get; set; }

        public int IdCompra { get; set; }
        public int IdProducto { get; set; }

        // Propiedades del detalle
        public int Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal Subtotal { get; set; } // Cantidad * CostoUnitario
    }
}