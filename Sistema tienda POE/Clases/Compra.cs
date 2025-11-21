using System;
using System.Collections.Generic; // Añadido por si se usa en el futuro para DetalleCompra

namespace Sistema_tienda_POE.Clases
{
    /// <summary>
    /// Modelo para mapear la tabla [dbo].[Compra].
    /// Esta es la cabecera de la transacción de compra.
    /// </summary>
    public class Compra
    {
        // Clave Primaria (PK) - Debe ser la columna IDENTITY en la BD
        public int IdCompra { get; set; }

        // Claves Foráneas (FK)
        public int IdProveedor { get; set; }
        public int IdUsuario { get; set; } // Usuario que registra la compra

        // Propiedades del Encabezado
        public DateTime Fecha { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
        public bool Estado { get; set; } // true (activa) / false (anulada)
        public string Observacion { get; set; }

        // Opcional: Propiedad para el detalle (no mapeada directamente a la BD, pero útil)
        // public List<DetalleCompra> Detalles { get; set; }
    }
}