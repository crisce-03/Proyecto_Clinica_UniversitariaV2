using Microsoft.AspNetCore.Mvc;
using Proyecto_Clinica_Universitaria.Datos;
using Proyecto_Clinica_Universitaria.Models;

namespace Proyecto_Clinica_Universitaria.Controllers
{
    public class PacienteController : Controller
    {
        // GET: Paciente/Index
        public IActionResult Index(int? id)
        {
            var datos = new PacienteDatos();
            var viewModel = new PacienteVistaModel
            {
                ListaPacientes = datos.ListarPacientes()
            };

            if (id != null && id > 0)
            {
                // Cargar paciente para editar
                var paciente = datos.ObtenerPacientePorIdCompleto(id.Value);
                if (paciente != null)
                {
                    viewModel.NuevoPaciente = paciente;
                }
                else
                {
                    TempData["Error"] = "Paciente no encontrado.";
                }
            }

            return View(viewModel);
        }

        // POST: Paciente/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Guardar(PacienteVistaModel model)
        {
            if (!ModelState.IsValid)
            {
                var datos = new PacienteDatos();
                model.ListaPacientes = datos.ListarPacientes();
                return View("Index", model);
            }

            var datosPaciente = new PacienteDatos();

            if (model.NuevoPaciente.Codigo == 0)
            {
                // Guardar nuevo paciente
                datosPaciente.GuardarPaciente(model.NuevoPaciente);
            }
            else
            {
                // Actualizar paciente existente
                datosPaciente.ActualizarPaciente(model.NuevoPaciente);
            }

            return RedirectToAction("Index");
        }

        // GET: Paciente/ObtenerPacienteJson
        [HttpGet]
        public JsonResult ObtenerPacienteJson(int codigo)
        {
            var datos = new PacienteDatos();
            var paciente = datos.ObtenerPacientePorIdCompleto(codigo); // Devuelve todos los campos

            if (paciente == null) return Json(null);

            // Evitar valores null para que JS no muestre "undefined"
            var pacienteSeguro = new
            {
                Codigo = paciente.Codigo,
                Cedula = paciente.Cedula ?? string.Empty,
                Nombre = paciente.Nombre ?? string.Empty,
                Apellido = paciente.Apellido ?? string.Empty,
                Sexo = paciente.Sexo ?? "Femenino",
                Edad = paciente.Edad,
                Ocupacion = paciente.Ocupacion ?? string.Empty,
                EstadoCivil = paciente.EstadoCivil ?? "Soltero/a",
                Domicilio = paciente.Domicilio ?? string.Empty,
                Telefono = paciente.Telefono ?? string.Empty,
                Estado = paciente.Estado ?? "Activo"
            };

            return Json(pacienteSeguro);
        }

        // POST: Paciente/Eliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Eliminar([FromForm] int codigo)
        {
            if (codigo <= 0) return Json(new { success = false });

            var datos = new PacienteDatos();
            bool resultado = datos.EliminarPaciente(codigo);

            return Json(new { success = resultado });
        }

        [HttpGet]
        public JsonResult ObtenerPaciente(int id)
        {
            var datos = new PacienteDatos();
            var p = datos.ObtenerPacientePorId(id); // tu método de datos
            if (p == null) return Json(null);

            // Forzamos nombres esperados por el JS
            return Json(new { codigo = p.Codigo, cedula = p.Cedula, nombre = p.Nombre, apellido = p.Apellido });
        }

    }
}
