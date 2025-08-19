using Microsoft.AspNetCore.Mvc;
using Proyecto_Clinica_Universitaria.Models;
using Proyecto_Clinica_Universitaria.Datos;
using System.Data;

namespace Proyecto_Clinica_Universitaria.Controllers
{
    public class MedicosEspecialidadController : Controller
    {
        MedicosEspecialidadDatos _MedicoEspecialidad = new MedicosEspecialidadDatos();

        public IActionResult Index()
        {
            // Cargar la lista completa de especialidades para la tabla
            ViewBag.ListaEspecialidades = _MedicoEspecialidad.Listar();
            return View(new MedicosEspecialidadModel()); // Devolver un modelo vacío para el formulario
        }

        [HttpPost]
        public IActionResult GuardarOEditar(MedicosEspecialidadModel especialidadmedico)
        {
            bool respuesta;

            if (especialidadmedico.Codigo == 0)
            {
                // Es un nuevo registro, por lo que se guarda
                respuesta = _MedicoEspecialidad.Guardar(especialidadmedico);
            }
            else
            {
                // El registro ya existe, por lo que se edita
                respuesta = _MedicoEspecialidad.Editar(especialidadmedico);
            }

            if (respuesta)
            {
                return RedirectToAction("Index");
            }
            else
            {
                // Si hay un error, recargar la vista con el modelo actual
                ViewBag.ListaEspecialidades = _MedicoEspecialidad.Listar();
                return View("Index", especialidadmedico);
            }
        }

        [HttpPost]
        public IActionResult Eliminar(int codigo)
        {
            var respuesta = _MedicoEspecialidad.Eliminar(codigo);

            if (respuesta)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ListaEspecialidades = _MedicoEspecialidad.Listar();
                return View("Index", new MedicosEspecialidadModel());
            }
        }
    }
}