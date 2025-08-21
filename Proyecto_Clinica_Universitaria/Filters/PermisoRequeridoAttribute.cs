using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Proyecto_Clinica_Universitaria.Filtros
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class PermisoRequeridoAttribute : ActionFilterAttribute
    {
        private readonly string[] _permitidos;

        /// <summary>
        /// Permite uno o más permisos: "Lectura", "Edicion", "Administracion"
        /// Ej: [PermisoRequerido("Administracion")] o [PermisoRequerido("Edicion","Administracion")]
        /// </summary>
        public PermisoRequeridoAttribute(params string[] permitidos)
        {
            _permitidos = permitidos ?? Array.Empty<string>();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var http = context.HttpContext;
            var permisoActual = http.Session.GetString("Permiso") ?? "Lectura";

            // Si no hay restricciones, no bloqueamos
            if (_permitidos.Length == 0)
            {
                base.OnActionExecuting(context);
                return;
            }

            // ¿El permiso actual está dentro de los permitidos?
            var ok = Array.Exists(_permitidos, p =>
                string.Equals(p, permisoActual, StringComparison.OrdinalIgnoreCase));

            if (!ok)
            {
                // Opción 1: 403 Forbidden
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);

                // Opción 2 (alternativa): redirigir al login o a una página de “sin permiso”
                // context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}

