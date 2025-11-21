namespace Sistema_tienda_POE.Clases
{
    public class Proveedor
    {
        public int IdProveedor { get; set; }
        public string Nombre { get; set; }

        public string NIT { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }

        public bool Estado { get; set; }
    }
}