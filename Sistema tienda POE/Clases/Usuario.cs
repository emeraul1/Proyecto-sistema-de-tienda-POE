using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_tienda_POE.Clases
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Contraseña { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public int IdRol { get; set; }
        public bool Estado { get; set; }
        public DateTime  FechaRegistro {get;set;}

        public int IdEmpleado { get; set; } // se agrego para relacionar usuario con empleado
        public string NombreRol { get; set; }  // se agrego para mostar en dgv la columna rol el nombre del rol
    }
}
