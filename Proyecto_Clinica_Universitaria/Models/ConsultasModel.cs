using Microsoft.Data.SqlClient;

namespace Proyecto_Clinica_Universitaria.Models
{
    public class ConsultasModel
    {
        public int Codigo { get; set; }
        public int Paciente { get; set; }
        public string? Cedula { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public DateTime Fecha { get; set; }
        public string? Temperatura { get; set; }
        public string? Pulso { get; set; }
        public string? Peso { get; set; }
        public string? Presion { get; set; }
        public string? SwatO2 { get; set; }
        public string? Recordatorio { get; set; }
        public string? Evolucion { get; set; }
        public string? Prescripcion { get; set; }
        public int Medico { get; set; }
        public int Medicamento { get; set; }
    }
}
