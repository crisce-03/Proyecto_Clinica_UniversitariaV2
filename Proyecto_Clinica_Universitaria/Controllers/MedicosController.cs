using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Clinica_Universitaria.Datos;
using Proyecto_Clinica_Universitaria.Models;
using Proyecto_Clinica_Universitaria.Servicios;

public class MedicosController : Controller
{
    private readonly MedicoDatos _medicoDatos = new MedicoDatos();
    private readonly MedicosEspecialidadDatos _especialidadDatos = new MedicosEspecialidadDatos();
    private readonly AzureBlobService _blob;

    public MedicosController(AzureBlobService blob)
    {
        _blob = blob;
    }

    public IActionResult Index()
    {
        ViewBag.ListaMedicos = _medicoDatos.Listar();
        ViewBag.ListaEspecialidades = _especialidadDatos.Listar();
        return View(new MedicoModel());
    }

    [HttpGet]
    public IActionResult Editar(int codigo)
    {
        var medico = _medicoDatos.Obtener(codigo);
        if (medico == null)
        {
            TempData["Mensaje"] = "No se encontró el médico solicitado.";
            return RedirectToAction("Index");
        }

        ViewBag.ListaMedicos = _medicoDatos.Listar();
        ViewBag.ListaEspecialidades = _especialidadDatos.Listar();
        return View("Index", medico);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(10_000_000)] // ~10 MB; ajusta si necesitas más/menos
    public async Task<IActionResult> GuardarOEditar(MedicoModel modelo, IFormFile? imagenArchivo)
    {
        try
        {
            // Si subieron un archivo, lo validamos y lo subimos a Azure Blob
            if (imagenArchivo != null && imagenArchivo.Length > 0)
            {
                var ext = Path.GetExtension(imagenArchivo.FileName).ToLowerInvariant();
                var permitidas = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };

                if (!permitidas.Contains(ext))
                {
                    ModelState.AddModelError(nameof(MedicoModel.ImagenMedico),
                        "Formato no permitido. Usa .jpg, .jpeg, .png, .gif, .webp o .bmp");

                    ViewBag.ListaMedicos = _medicoDatos.Listar();
                    ViewBag.ListaEspecialidades = _especialidadDatos.Listar();
                    return View("Index", modelo);
                }

                if (imagenArchivo.Length > 8 * 1024 * 1024) // 8 MB
                {
                    ModelState.AddModelError(nameof(MedicoModel.ImagenMedico),
                        "La imagen excede el tamaño máximo de 8 MB.");

                    ViewBag.ListaMedicos = _medicoDatos.Listar();
                    ViewBag.ListaEspecialidades = _especialidadDatos.Listar();
                    return View("Index", modelo);
                }

                using var stream = imagenArchivo.OpenReadStream();
                var contentType = string.IsNullOrWhiteSpace(imagenArchivo.ContentType)
                    ? "application/octet-stream"
                    : imagenArchivo.ContentType;

                // Subida a Azure: devuelve URL pública
                var (_, urlPublica) = await _blob.UploadAsync(stream, imagenArchivo.FileName, contentType);

                // Guardaremos SOLO la URL en tu modelo (tu BD ya espera string)
                modelo.ImagenMedico = urlPublica;
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ListaMedicos = _medicoDatos.Listar();
                ViewBag.ListaEspecialidades = _especialidadDatos.Listar();
                return View("Index", modelo);
            }

            bool ok = (modelo.Codigo == 0)
                ? _medicoDatos.Guardar(modelo)
                : _medicoDatos.Editar(modelo);

            TempData["Mensaje"] = ok ? "Guardado correctamente." : "Ocurrió un error al guardar.";
        }
        catch (Exception ex)
        {
            TempData["Mensaje"] = ex.Message;
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Eliminar(int codigo)
    {
        // Con tu modelo actual solo guardamos la URL, no el nombre del blob.
        // Para mantenerlo simple, eliminamos solo en BD.
        // (Si más adelante quieres borrar también en Azure, podemos derivar el blobName desde la URL.)
        bool ok = _medicoDatos.Eliminar(codigo);
        TempData["Mensaje"] = ok ? "Registro eliminado." : "Ocurrió un error al eliminar.";
        return RedirectToAction("Index");
    }
}


