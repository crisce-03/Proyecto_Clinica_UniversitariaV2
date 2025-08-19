using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Proyecto_Clinica_Universitaria.Models;

namespace Proyecto_Clinica_Universitaria.Datos
{
    public class MedicoDatos
    {
        // Helper seguro: intenta leer una columna; si no existe, devuelve null
        private static string? TryGetString(IDataRecord dr, string col)
        {
            try
            {
                int i = ((IDataRecord)dr).GetOrdinal(col);
                return dr.IsDBNull(i) ? null : dr.GetValue(i)?.ToString();
            }
            catch (IndexOutOfRangeException)
            {
                return null; // la columna no viene en el SELECT
            }
        }

        public List<MedicoModel> Listar()
        {
            var lista = new List<MedicoModel>();
            var cn = new Conexion();

            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ListarMedico", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new MedicoModel
                            {
                                Codigo = Convert.ToInt32(dr["Codigo"]),
                                Cedula = dr["Cedula"].ToString() ?? string.Empty,
                                Nombre = dr["Nombre"].ToString() ?? string.Empty,
                                Apellido = dr["Apellido"].ToString() ?? string.Empty,
                                EspecialidadCodigo = Convert.ToInt32(dr["EspecialidadCodigo"]),
                                Telefono = dr["Telefono"] == DBNull.Value ? null : dr["Telefono"]!.ToString(),
                                Correo = dr["Correo"] == DBNull.Value ? null : dr["Correo"]!.ToString(),
                                Usuario = dr["Usuario"] == DBNull.Value ? null : dr["Usuario"]!.ToString(),

                                // ❌ NO LEAS dr["Contrasena"] si el SP no la trae
                                // Contrasena = ...

                                // bit -> "Activo"/"Pasivo"
                                Estado = (dr["Estado"] != DBNull.Value && Convert.ToBoolean(dr["Estado"])) ? "Activo" : "Pasivo",

                                ImagenMedico = dr["ImagenMedico"] == DBNull.Value ? null : dr["ImagenMedico"]!.ToString()
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public MedicoModel? Obtener(int codigo)
        {
            MedicoModel? obj = null;
            var cn = new Conexion();

            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerMedico", conexion))
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
                                Cedula = dr["Cedula"].ToString() ?? string.Empty,
                                Nombre = dr["Nombre"].ToString() ?? string.Empty,
                                Apellido = dr["Apellido"].ToString() ?? string.Empty,
                                EspecialidadCodigo = Convert.ToInt32(dr["EspecialidadCodigo"]),
                                Telefono = dr["Telefono"] == DBNull.Value ? null : dr["Telefono"]!.ToString(),
                                Correo = dr["Correo"] == DBNull.Value ? null : dr["Correo"]!.ToString(),
                                Usuario = dr["Usuario"] == DBNull.Value ? null : dr["Usuario"]!.ToString(),

                                // Si algún día la traes en el SP, la tomamos; si no, queda null
                                Contrasena = TryGetString(dr, "Contrasena"),

                                Estado = (dr["Estado"] != DBNull.Value && Convert.ToBoolean(dr["Estado"])) ? "Activo" : "Pasivo",
                                ImagenMedico = dr["ImagenMedico"] == DBNull.Value ? null : dr["ImagenMedico"]!.ToString()
                            };
                        }
                    }
                }
            }

            return obj;
        }

        public bool Guardar(MedicoModel obj)
        {
            bool result;
            var cn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(cn.getCadenaSQL()))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_GuardarMedico", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Cedula", obj.Cedula);
                        cmd.Parameters.AddWithValue("@Nombre", obj.Nombre);
                        cmd.Parameters.AddWithValue("@Apellido", obj.Apellido);
                        cmd.Parameters.AddWithValue("@EspecialidadCodigo", obj.EspecialidadCodigo);
                        cmd.Parameters.AddWithValue("@Telefono", (object?)obj.Telefono ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Correo", (object?)obj.Correo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Usuario", (object?)obj.Usuario ?? DBNull.Value);

                        // IMPORTANTE: tu tabla tiene Contrasena NOT NULL
                        // Asegúrate de que venga algo; si dejas null, fallará.
                        cmd.Parameters.AddWithValue("@Contrasena", (object?)obj.Contrasena ?? ""); // <- mejor valida antes

                        // "Activo"/"Pasivo"/"true"/"false"/"1"/"0" -> bit
                        bool estadoBit =
                            string.Equals(obj.Estado, "Activo", StringComparison.OrdinalIgnoreCase) ||
                            obj.Estado == "1" ||
                            string.Equals(obj.Estado, "true", StringComparison.OrdinalIgnoreCase);
                        cmd.Parameters.Add("@Estado", SqlDbType.Bit).Value = estadoBit;

                        cmd.Parameters.AddWithValue("@ImagenMedico", (object?)obj.ImagenMedico ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
                result = true;
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new Exception("La Cédula o el Usuario ya existen. Usa valores distintos.", ex);
            }
            catch
            {
                throw;
            }

            return result;
        }

        public bool Editar(MedicoModel obj)
        {
            bool result;
            var cn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(cn.getCadenaSQL()))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_EditarMedico", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Codigo", obj.Codigo);
                        cmd.Parameters.AddWithValue("@Cedula", obj.Cedula);
                        cmd.Parameters.AddWithValue("@Nombre", obj.Nombre);
                        cmd.Parameters.AddWithValue("@Apellido", obj.Apellido);
                        cmd.Parameters.AddWithValue("@EspecialidadCodigo", obj.EspecialidadCodigo);
                        cmd.Parameters.AddWithValue("@Telefono", (object?)obj.Telefono ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Correo", (object?)obj.Correo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Usuario", (object?)obj.Usuario ?? DBNull.Value);

                        // OJO: si no quieres sobreescribir con vacío, ver nota de SP abajo
                        cmd.Parameters.AddWithValue("@Contrasena", (object?)obj.Contrasena ?? "");

                        bool estadoBit =
                            string.Equals(obj.Estado, "Activo", StringComparison.OrdinalIgnoreCase) ||
                            obj.Estado == "1" ||
                            string.Equals(obj.Estado, "true", StringComparison.OrdinalIgnoreCase);
                        cmd.Parameters.Add("@Estado", SqlDbType.Bit).Value = estadoBit;

                        cmd.Parameters.AddWithValue("@ImagenMedico", (object?)obj.ImagenMedico ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
                result = true;
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new Exception("La Cédula o el Usuario ya existen. Usa valores distintos.", ex);
            }
            catch
            {
                throw;
            }

            return result;
        }

        public bool Eliminar(int codigo)
        {
            bool result;
            var cn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(cn.getCadenaSQL()))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_EliminarMedico", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Codigo", codigo);
                        cmd.ExecuteNonQuery();
                    }
                }
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public List<MedicoModel> ListarNombres()
        {
            var lista = new List<MedicoModel>();
            var cn = new Conexion();

            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();
                // En tu BD la tabla es "Medico" (singular)
                using (SqlCommand cmd = new SqlCommand("SELECT Codigo, Nombre FROM Medico", conexion))
                {
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
            }

            return lista;
        }
    }
}
