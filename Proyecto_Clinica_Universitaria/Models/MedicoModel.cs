using Microsoft.Data.SqlClient;

namespace Proyecto_Clinica_Universitaria.Models
{
    public class MedicoModel
    {
        public int Codigo { get; set; }

        // En la BD es VARCHAR(20)
        public string Cedula { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Apellido { get; set; } = string.Empty;

        // FK a EspecialidadMedico(Codigo)
        public int EspecialidadCodigo { get; set; }

        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? Usuario { get; set; }

        // En la BD se llama Contrasena
        public string? Contrasena { get; set; }

        // Activo | Pasivo
        public string Estado { get; set; } = "Activo";

        // Solo para mostrar (no se envía al SP)
        public string? Especialidad { get; set; }

        public string? ImagenMedico { get; set; }

    }
}
