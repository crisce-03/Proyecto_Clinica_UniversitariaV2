using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Proyecto_Clinica_Universitaria.Models;

namespace Proyecto_Clinica_Universitaria.Datos
{
    public class MedicoDatos
    {
        // Helper: intenta leer una columna; si no existe, devuelve null
        private static string? TryGetString(IDataRecord dr, string col)
        {
            try
            {
                int i = dr.GetOrdinal(col);
                return dr.IsDBNull(i) ? null : dr.GetValue(i)?.ToString();
            }
            catch (IndexOutOfRangeException)
            {
                return null; // la columna no viene en el SELECT/SP
            }
        }

        private static string NormalizarPermiso(string? p)
        {
            var v = (p ?? "").Trim();
            if (v.Equals("Administracion", StringComparison.OrdinalIgnoreCase)) return "Administracion";
            if (v.Equals("Edicion", StringComparison.OrdinalIgnoreCase)) return "Edicion";
            return "Lectura";
        }

        private static object DbNullIfNullOrEmpty(string? v) =>
            string.IsNullOrWhiteSpace(v) ? DBNull.Value : v!;

        // =========================
        // LISTAR
        // =========================
        public List<MedicoModel> Listar()
        {
            var lista = new List<MedicoModel>();
            var cn = new Conexion();

            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_ListarMedico", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new MedicoModel
                            {
                                Codigo = Convert.ToInt32(dr["Codigo"]),
                                Cedula = dr["Cedula"]?.ToString() ?? string.Empty,
                                Nombre = dr["Nombre"]?.ToString() ?? string.Empty,
                                Apellido = dr["Apellido"]?.ToString() ?? string.Empty,
                                EspecialidadCodigo = Convert.ToInt32(dr["EspecialidadCodigo"]),
                                Telefono = dr["Telefono"] == DBNull.Value ? null : dr["Telefono"]!.ToString(),
                                Correo = dr["Correo"] == DBNull.Value ? null : dr["Correo"]!.ToString(),
                                Usuario = dr["Usuario"] == DBNull.Value ? null : dr["Usuario"]!.ToString(),
                                // Contrasena: normalmente no se lista
                                ImagenMedico = dr["ImagenMedico"] == DBNull.Value ? null : dr["ImagenMedico"]!.ToString(),
                                // 👇 Toma el permiso si viene; si no, por defecto "Lectura"
                                Permiso = TryGetString(dr, "Permiso") ?? "Lectura"
                            });
                        }
                    }
                }
            }
            return lista;
        }

        // =========================
        // OBTENER (por codigo)
        // =========================
        public MedicoModel? Obtener(int codigo)
        {
            MedicoModel? obj = null;
            var cn = new Conexion();

            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_ObtenerMedico", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Codigo", codigo);

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            obj = new MedicoModel
                            {
                                Codigo = Convert.ToInt32(dr["Codigo"]),
                                Cedula = dr["Cedula"]?.ToString() ?? string.Empty,
                                Nombre = dr["Nombre"]?.ToString() ?? string.Empty,
                                Apellido = dr["Apellido"]?.ToString() ?? string.Empty,
                                EspecialidadCodigo = Convert.ToInt32(dr["EspecialidadCodigo"]),
                                Telefono = dr["Telefono"] == DBNull.Value ? null : dr["Telefono"]!.ToString(),
                                Correo = dr["Correo"] == DBNull.Value ? null : dr["Correo"]!.ToString(),
                                Usuario = dr["Usuario"] == DBNull.Value ? null : dr["Usuario"]!.ToString(),
                                Contrasena = TryGetString(dr, "Contrasena"), // si el SP la trae
                                ImagenMedico = dr["ImagenMedico"] == DBNull.Value ? null : dr["ImagenMedico"]!.ToString(),
                                // 👇 Permiso si lo trae el SP; si no, "Lectura"
                                Permiso = TryGetString(dr, "Permiso") ?? "Lectura"
                            };
                        }
                    }
                }
            }
            return obj;
        }

        // =========================
        // GUARDAR (INSERT)
        // =========================
        public bool Guardar(MedicoModel obj)
        {
            var cn = new Conexion();

            // Normaliza el permiso antes de enviar a la BD
            obj.Permiso = NormalizarPermiso(obj.Permiso);

            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_GuardarMedico", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Cedula", obj.Cedula ?? "");
                    cmd.Parameters.AddWithValue("@Nombre", obj.Nombre ?? "");
                    cmd.Parameters.AddWithValue("@Apellido", obj.Apellido ?? "");
                    cmd.Parameters.AddWithValue("@EspecialidadCodigo", obj.EspecialidadCodigo);
                    cmd.Parameters.AddWithValue("@Telefono", DbNullIfNullOrEmpty(obj.Telefono));
                    cmd.Parameters.AddWithValue("@Correo", DbNullIfNullOrEmpty(obj.Correo));
                    cmd.Parameters.AddWithValue("@Usuario", DbNullIfNullOrEmpty(obj.Usuario));

                    // IMPORTANTE: si Contrasena es NOT NULL en la BD, asegúrate que venga con valor.
                    cmd.Parameters.AddWithValue("@Contrasena", string.IsNullOrWhiteSpace(obj.Contrasena) ? "" : obj.Contrasena);

                    cmd.Parameters.AddWithValue("@ImagenMedico", DbNullIfNullOrEmpty(obj.ImagenMedico));

                    // 👇 Nuevo parámetro @Permiso en el SP
                    cmd.Parameters.AddWithValue("@Permiso", obj.Permiso);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
                    {
                        // Claves duplicadas (cedula/usuario únicos)
                        throw new Exception("La Cédula o el Usuario ya existen. Usa valores distintos.", ex);
                    }
                }
            }
        }

        // =========================
        // EDITAR (UPDATE)
        // =========================
        public bool Editar(MedicoModel obj)
        {
            var cn = new Conexion();

            obj.Permiso = NormalizarPermiso(obj.Permiso);

            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_EditarMedico", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Codigo", obj.Codigo);
                    cmd.Parameters.AddWithValue("@Cedula", obj.Cedula ?? "");
                    cmd.Parameters.AddWithValue("@Nombre", obj.Nombre ?? "");
                    cmd.Parameters.AddWithValue("@Apellido", obj.Apellido ?? "");
                    cmd.Parameters.AddWithValue("@EspecialidadCodigo", obj.EspecialidadCodigo);
                    cmd.Parameters.AddWithValue("@Telefono", DbNullIfNullOrEmpty(obj.Telefono));
                    cmd.Parameters.AddWithValue("@Correo", DbNullIfNullOrEmpty(obj.Correo));
                    cmd.Parameters.AddWithValue("@Usuario", DbNullIfNullOrEmpty(obj.Usuario));

                    // Si NO quieres sobreescribir la contraseña cuando venga vacía,
                    // envía NULL y en el SP usa: Contrasena = COALESCE(@Contrasena, Contrasena)
                    var contrasenaParam = string.IsNullOrWhiteSpace(obj.Contrasena)
                        ? (object)DBNull.Value
                        : obj.Contrasena!;
                    cmd.Parameters.AddWithValue("@Contrasena", contrasenaParam);

                    cmd.Parameters.AddWithValue("@ImagenMedico", DbNullIfNullOrEmpty(obj.ImagenMedico));

                    // 👇 Nuevo parámetro @Permiso en el SP de edición
                    cmd.Parameters.AddWithValue("@Permiso", obj.Permiso);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
                    {
                        throw new Exception("La Cédula o el Usuario ya existen. Usa valores distintos.", ex);
                    }
                }
            }
        }

        // =========================
        // ELIMINAR
        // =========================
        public bool Eliminar(int codigo)
        {
            var cn = new Conexion();

            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_EliminarMedico", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Codigo", codigo);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }

        // =========================
        // LISTAR SOLO NOMBRES (utilidad)
        // =========================
        public List<MedicoModel> ListarNombres()
        {
            var lista = new List<MedicoModel>();
            var cn = new Conexion();

            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("SELECT Codigo, Nombre FROM Medico", conexion))
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new MedicoModel
                        {
                            Codigo = dr.GetInt32(0),
                            Nombre = dr.GetString(1)
                        });
                    }
                }
            }
            return lista;
        }

        public MedicoModel? Autenticar(string usuario, string contrasena)
        {
            var cn = new Conexion();
            using var conexion = new SqlConnection(cn.getCadenaSQL());
            conexion.Open();

            // Opción rápida con SELECT directo
            // (recomendación futura: guardar contraseñas con hash y comparar hash aquí)
            using var cmd = new SqlCommand(@"
        SELECT TOP 1
            Codigo, Usuario, Contrasena, Nombre, Apellido, Permiso, ImagenMedico
        FROM Medico
        WHERE Usuario = @u AND Contrasena = @p
          -- AND Estado = 'Activo'   -- <- descomenta si aplicas estado
    ", conexion);

            cmd.Parameters.AddWithValue("@u", usuario);
            cmd.Parameters.AddWithValue("@p", contrasena);

            using var dr = cmd.ExecuteReader();
            if (!dr.Read()) return null;

            return new MedicoModel
            {
                Codigo = Convert.ToInt32(dr["Codigo"]),
                Usuario = dr["Usuario"]?.ToString(),
                // Contrasena = dr["Contrasena"]?.ToString(), // normalmente no hace falta devolverla
                Nombre = dr["Nombre"]?.ToString(),
                Apellido = dr["Apellido"]?.ToString(),
                Permiso = dr["Permiso"]?.ToString() ?? "Lectura",
                ImagenMedico = dr["ImagenMedico"] == DBNull.Value ? null : dr["ImagenMedico"]!.ToString()
            };
        }


    }
}
