using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Clinica_Universitaria.Datos;
using Proyecto_Clinica_Universitaria.Filtros;
using Proyecto_Clinica_Universitaria.Models;
using Proyecto_Clinica_Universitaria.Servicios;

namespace Proyecto_Clinica_Universitaria.Controllers
{
    public class PacienteController : Controller
    {
        private readonly AzureBlobService _blob;

        public PacienteController(AzureBlobService blob)
        {
            _blob = blob;
        }

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
                var paciente = datos.ObtenerPacientePorIdCompleto(id.Value);
                if (paciente != null) viewModel.NuevoPaciente = paciente;
                else TempData["Error"] = "Paciente no encontrado.";
            }

            return View(viewModel);
        }

        // POST: Paciente/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(10_000_000)] // ~10MB
        [PermisoRequerido("Edicion", "Administracion")]
        public async Task<IActionResult> Guardar(PacienteVistaModel model, IFormFile? imagenArchivo)
        {
            // Subida de imagen (opcional)
            try
            {
                if (imagenArchivo != null && imagenArchivo.Length > 0)
                {
                    var ext = Path.GetExtension(imagenArchivo.FileName).ToLowerInvariant();
                    var permitidas = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                        { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };

                    if (!permitidas.Contains(ext))
                        ModelState.AddModelError("NuevoPaciente.ImagenPaciente", "Formato no permitido. Usa .jpg, .jpeg, .png, .gif, .webp o .bmp");

                    if (imagenArchivo.Length > 8 * 1024 * 1024) // 8MB
                        ModelState.AddModelError("NuevoPaciente.ImagenPaciente", "La imagen excede el tamaño máximo de 8 MB.");

                    if (!ModelState.IsValid)
                    {
                        var datos = new PacienteDatos();
                        model.ListaPacientes = datos.ListarPacientes();
                        return View("Index", model);
                    }

                    using var stream = imagenArchivo.OpenReadStream();
                    var contentType = string.IsNullOrWhiteSpace(imagenArchivo.ContentType)
                        ? "application/octet-stream"
                        : imagenArchivo.ContentType;

                    var (_, urlPublica) = await _blob.UploadAsync(stream, imagenArchivo.FileName, contentType);

                    // Guardamos solo la URL
                    model.NuevoPaciente.ImagenPaciente = urlPublica;
                }
                // Si no se sube archivo y viene ya una URL en el input de texto, se queda tal cual
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al subir imagen: {ex.Message}";
            }

            if (!ModelState.IsValid)
            {
                var datos = new PacienteDatos();
                model.ListaPacientes = datos.ListarPacientes();
                return View("Index", model);
            }

            var datosPaciente = new PacienteDatos();
            if (model.NuevoPaciente.Codigo == 0)
                datosPaciente.GuardarPaciente(model.NuevoPaciente);
            else
                datosPaciente.ActualizarPaciente(model.NuevoPaciente);

            TempData["Mensaje"] = "Guardado correctamente.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult ObtenerPacienteJson(int codigo)
        {
            var datos = new PacienteDatos();
            var paciente = datos.ObtenerPacientePorIdCompleto(codigo);
            if (paciente == null) return Json(null);

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
                Estado = paciente.Estado ?? "Activo",
                ImagenPaciente = paciente.ImagenPaciente ?? string.Empty // NUEVO
            };
            return Json(pacienteSeguro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Eliminar([FromForm] int codigo)
        {
            if (codigo <= 0) return Json(new { success = false });
            var datos = new PacienteDatos();
            // Nota: aquí solo eliminamos en BD (igual que Médicos).
            var ok = datos.EliminarPaciente(codigo);
            return Json(new { success = ok });
        }

        [HttpGet]
        public JsonResult ObtenerPaciente(int id)
        {
            var datos = new PacienteDatos();
            var p = datos.ObtenerPacientePorId(id);
            if (p == null) return Json(null);
            return Json(new { codigo = p.Codigo, cedula = p.Cedula, nombre = p.Nombre, apellido = p.Apellido });
        }
    }
}
