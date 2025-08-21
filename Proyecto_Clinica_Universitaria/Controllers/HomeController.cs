using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Clinica_Universitaria.Datos;  // 👈 importa tu capa de datos
using Proyecto_Clinica_Universitaria.Models;

namespace Proyecto_Clinica_Universitaria.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MedicoDatos _medicoDatos;

        public HomeController(ILogger<HomeController> logger, MedicoDatos medicoDatos)
        {
            _logger = logger;
            _medicoDatos = medicoDatos;
        }

        // GET: /Home/Index -> login
        [HttpGet]
        public IActionResult Index()
        {
            // Si ya inició sesión, reubica según su permiso
            var permiso = HttpContext.Session.GetString("Permiso");
            if (!string.IsNullOrEmpty(permiso))
                return RedirigirPorPermiso(permiso);

            return View();
        }

        // POST: /Home/Index -> procesa login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string usuario, string contrasena)
        {
            // 1) Admin hardcodeado
            if (string.Equals(usuario, "Administrador", StringComparison.OrdinalIgnoreCase) &&
                contrasena == "caca")
            {
                HttpContext.Session.SetString("Usuario", "Administrador");
                HttpContext.Session.SetString("Nombre", "Administrador del sistema");
                HttpContext.Session.SetString("Permiso", "Administracion");
                // puedes setear un MedicoCodigo ficticio si quieres
                return RedirectToAction("Index", "Medicos");
            }

            // 2) Intentar autenticar contra la BD (tabla Medico)
            var medico = _medicoDatos.Autenticar(usuario?.Trim() ?? "", contrasena ?? "");
            if (medico == null)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos.";
                return View();
            }

            // Guardar sesión
            HttpContext.Session.SetString("Usuario", medico.Usuario ?? usuario);
            HttpContext.Session.SetString("Nombre", $"{medico.Nombre} {medico.Apellido}".Trim());
            HttpContext.Session.SetString("Permiso", medico.Permiso ?? "Lectura");
            HttpContext.Session.SetInt32("MedicoCodigo", medico.Codigo);

            // Redirigir según permiso del médico
            return RedirigirPorPermiso(medico.Permiso);
        }

        private IActionResult RedirigirPorPermiso(string? permiso)
        {
            switch ((permiso ?? "Lectura").Trim())
            {
                case "Administracion":
                    return RedirectToAction("Index", "Medicos");
                case "Edicion":
                    return RedirectToAction("Index", "Paciente");
                default: // Lectura
                    return RedirectToAction("HomePage");
            }
        }

        public IActionResult HomePage() => View();

        public IActionResult Privacy() => View();

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

