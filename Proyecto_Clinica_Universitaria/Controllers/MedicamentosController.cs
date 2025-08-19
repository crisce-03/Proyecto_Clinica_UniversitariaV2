using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Clinica_Universitaria.Datos;
using Proyecto_Clinica_Universitaria.Models;

// 👇 Cambia por el namespace real donde definiste AzureBlobService
using Proyecto_Clinica_Universitaria.Servicios;

namespace Proyecto_Clinica_Universitaria.Controllers
{
    public class MedicamentosController : Controller
    {
        private readonly MedicamentosDatos _datos = new MedicamentosDatos();
        private readonly AzureBlobService _blob;  // <-- Azure

        public MedicamentosController(AzureBlobService blob)
        {
            _blob = blob;
        }

        public IActionResult Index()
        {
            ViewBag.Lista = _datos.Listar();
            return View(new MedicamentoModel());
        }

        [HttpGet]
        public IActionResult Editar(int codigo)
        {
            var modelo = _datos.Obtener(codigo);
            if (modelo == null)
            {
                TempData["Mensaje"] = "No se encontró el medicamento.";
                return RedirectToAction("Index");
            }

            ViewBag.Lista = _datos.Listar();
            return View("Index", modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(10_000_000)] // ~10 MB
        public async Task<IActionResult> GuardarOEditar(MedicamentoModel modelo, IFormFile? imagenArchivo)
        {
            try
            {
                // Si suben archivo, se valida y se sube a Azure Blob (guardamos URL en modelo.Imagen)
                if (imagenArchivo != null && imagenArchivo.Length > 0)
                {
                    var ext = Path.GetExtension(imagenArchivo.FileName).ToLowerInvariant();
                    var permitidas = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                        { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };

                    if (!permitidas.Contains(ext))
                    {
                        ModelState.AddModelError(nameof(MedicamentoModel.Imagen),
                            "Formato de imagen no permitido. Usa .jpg, .jpeg, .png, .gif, .webp o .bmp");
                        ViewBag.Lista = _datos.Listar();
                        return View("Index", modelo);
                    }

                    if (imagenArchivo.Length > 8 * 1024 * 1024) // 8 MB
                    {
                        ModelState.AddModelError(nameof(MedicamentoModel.Imagen),
                            "La imagen excede el tamaño máximo de 8 MB.");
                        ViewBag.Lista = _datos.Listar();
                        return View("Index", modelo);
                    }

                    using var stream = imagenArchivo.OpenReadStream();
                    var contentType = string.IsNullOrWhiteSpace(imagenArchivo.ContentType)
                        ? "application/octet-stream"
                        : imagenArchivo.ContentType;

                    // Subir a Azure y obtener URL pública
                    var (_, urlPublica) = await _blob.UploadAsync(stream, imagenArchivo.FileName, contentType);

                    // Guardar SOLO la URL en tu campo de texto
                    modelo.Imagen = urlPublica;
                }

                bool ok = (modelo.Codigo == 0)
                    ? _datos.Guardar(modelo)
                    : _datos.Editar(modelo);

                TempData["Mensaje"] = ok ? "Guardado correctamente." : "Ocurrió un error al guardar.";
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int codigo)
        {
            try
            {
                // Con el modelo actual solo tenemos la URL; no se elimina el blob de Azure.
                // Si luego quieres borrar también en Azure, te doy una helper para derivar el blobName desde la URL.
                _datos.Eliminar(codigo);
                TempData["Mensaje"] = "Registro eliminado.";
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "No se pudo eliminar: " + ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}


