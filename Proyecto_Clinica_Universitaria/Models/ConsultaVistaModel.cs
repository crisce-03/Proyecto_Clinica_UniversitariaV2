namespace Proyecto_Clinica_Universitaria.Models
{
    public class ConsultaVistaModel
    {
        public int Codigo { get; set; }
        public DateTime Fecha { get; set; }
        public string? Cedula { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Temperatura { get; set; }
        public string? Pulso { get; set; }
        public string? Peso { get; set; }
        public string? Presion { get; set; }
        public string? SwatO2 { get; set; }
        public string? MedicoNombre { get; set; }
    }
}
