using System.Collections.Generic;

namespace Proyecto_Clinica_Universitaria.Models
{
    public class PacienteVistaModel
    {
        public PacienteModel NuevoPaciente { get; set; } = new PacienteModel();
        public List<PacienteModel> ListaPacientes { get; set; } = new List<PacienteModel>();
    }
}

