using Microsoft.AspNetCore.Mvc;
using Proyecto_Clinica_Universitaria.Datos;
using Proyecto_Clinica_Universitaria.Models;
using System.Collections.Generic;

namespace Proyecto_Clinica_Universitaria.Controllers
{
    public class HistorialConsultasController : Controller
    {
        private readonly ConsultasDatos _consultasDatos;

        public HistorialConsultasController(ConsultasDatos consultasDatos)
        {
            _consultasDatos = consultasDatos;
        }

        public IActionResult Index()
        {
            List<ConsultaVistaModel> listaConsultas = _consultasDatos.ListarConsultasConMedico() ?? new List<ConsultaVistaModel>();
            return View(listaConsultas);
        }
    }
}
