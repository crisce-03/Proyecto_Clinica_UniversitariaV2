using System;

namespace Proyecto_Clinica_Universitaria.Models
{
    public class MedicamentoModel
    {
        public int Codigo { get; set; }
        public string Medicamento { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int Cantidad { get; set; }
        public DateTime? Vencimiento { get; set; }
        public string? Imagen { get; set; } // ruta o nombre de archivo
    }
}

