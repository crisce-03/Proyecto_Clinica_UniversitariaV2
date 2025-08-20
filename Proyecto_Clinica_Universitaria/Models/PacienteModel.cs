namespace Proyecto_Clinica_Universitaria.Models
{
    public class PacienteModel
    {
        public int Codigo { get; set; }
        public string? Cedula { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Sexo { get; set; }
        public int Edad { get; set; }
        public string? Ocupacion { get; set; }
        public string? EstadoCivil { get; set; }
        public string? Domicilio { get; set; }
        public string? Telefono { get; set; }
        public string? Estado { get; set; }

        public string? ImagenPaciente { get; set; }
    }
}
