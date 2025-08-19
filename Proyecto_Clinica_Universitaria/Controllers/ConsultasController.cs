using Microsoft.AspNetCore.Mvc;
using Proyecto_Clinica_Universitaria.Models;
using Proyecto_Clinica_Universitaria.Datos;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Proyecto_Clinica_Universitaria.Controllers
{
    public class ConsultasController : Controller
    {
        private readonly ILogger<ConsultasController> _logger;
        private readonly ConsultasDatos _consultasDatos;
        private readonly PacienteDatos _pacienteDatos;
        private readonly MedicoDatos _medicoDatos;

        public ConsultasController(
            ILogger<ConsultasController> logger,
            ConsultasDatos consultasDatos,
            PacienteDatos pacienteDatos,
            MedicoDatos medicoDatos)
        {
            _logger = logger;
            _consultasDatos = consultasDatos;
            _pacienteDatos = pacienteDatos;
            _medicoDatos = medicoDatos;
        }

        // Vista que muestra listado de consultas (historial)
        public IActionResult Historial()
        {
            var lista = _consultasDatos.ListarConsultasConMedico();
            return View(lista); // Esta vista espera List<ConsultaVistaModel>
        }

        // Vista para crear o editar (si id es null, crea; si no, edita)
        public IActionResult Index(int? id)
        {
            ViewBag.ListaPacientes = _pacienteDatos.ListarNombres();
            ViewBag.ListaMedicos = _medicoDatos.ListarNombres();

            if (id == null)
            {
                return View(new ConsultasModel()); // Nuevo registro
            }
            else
            {
                var consulta = _consultasDatos.ObtenerPorCodigo(id.Value);
                if (consulta == null)
                {
                    return NotFound();
                }
                return View(consulta);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Guardar(ConsultasModel consulta)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ListaPacientes = _pacienteDatos.ListarNombres();
                ViewBag.ListaMedicos = _medicoDatos.ListarNombres();
                return View("Index", consulta);
            }

            bool resultado;

            if (consulta.Codigo == 0)
            {
                resultado = _consultasDatos.Guardar(consulta);
                _logger.LogInformation("Guardando nueva consulta");
            }
            else
            {
                resultado = _consultasDatos.Editar(consulta);
                _logger.LogInformation($"Editando consulta código {consulta.Codigo}");
            }

            if (!resultado)
            {
                _logger.LogError("Error al guardar/editar consulta.");
                ModelState.AddModelError("", "Ocurrió un error al guardar la consulta.");
                ViewBag.ListaPacientes = _pacienteDatos.ListarNombres();
                ViewBag.ListaMedicos = _medicoDatos.ListarNombres();
                return View("Index", consulta);
            }

            // Aquí está la modificación: redirigir a Index de HistorialConsultasController
            return RedirectToAction("Index", "HistorialConsultas");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar([FromForm] int codigo)
        {
            if (codigo <= 0)
            {
                return BadRequest(new { mensaje = "ID inválido." });
            }

            bool resultado = _consultasDatos.Eliminar(codigo);

            if (!resultado)
            {
                return NotFound(new { mensaje = "No se encontró la consulta." });
            }

            return Ok(new { mensaje = "Consulta eliminada correctamente." });
        }

    }
}