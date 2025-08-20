using Proyecto_Clinica_Universitaria.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Proyecto_Clinica_Universitaria.Datos
{
    public class PacienteDatos
    {
        // Listar todos los pacientes (incluye ImagenPaciente)
        public List<PacienteModel> ListarPacientes()
        {
            var lista = new List<PacienteModel>();
            var cn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(cn.getCadenaSQL()))
                {
                    conexion.Open();
                    using var cmd = new SqlCommand(
                        @"SELECT Codigo, Cedula, Nombre, Apellido, Sexo, Edad, Ocupacion, EstadoCivil, Domicilio, Telefono, Estado, ImagenPaciente
                          FROM Paciente", conexion);

                    using var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lista.Add(new PacienteModel
                        {
                            Codigo = dr.GetInt32(0),
                            Cedula = dr["Cedula"]?.ToString(),
                            Nombre = dr["Nombre"]?.ToString(),
                            Apellido = dr["Apellido"]?.ToString(),
                            Sexo = dr["Sexo"]?.ToString(),
                            Edad = Convert.ToInt32(dr["Edad"]),
                            Ocupacion = dr["Ocupacion"]?.ToString(),
                            EstadoCivil = dr["EstadoCivil"]?.ToString(),
                            Domicilio = dr["Domicilio"]?.ToString(),
                            Telefono = dr["Telefono"]?.ToString(),
                            Estado = dr["Estado"]?.ToString(),
                            ImagenPaciente = dr["ImagenPaciente"] == DBNull.Value ? null : dr["ImagenPaciente"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en ListarPacientes: {ex.Message}");
            }

            return lista;
        }

        public List<PacienteModel> ListarNombres()
        {
            var lista = new List<PacienteModel>();
            var cn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(cn.getCadenaSQL()))
                {
                    conexion.Open();
                    using var cmd = new SqlCommand("SELECT Codigo, Nombre FROM Paciente", conexion);

                    using var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lista.Add(new PacienteModel
                        {
                            Codigo = dr.GetInt32(0),
                            Nombre = dr.GetString(1)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en ListarNombres PacienteDatos: {ex.Message}");
            }

            return lista;
        }

        // Obtener paciente por Código (algunos campos)
        public PacienteModel ObtenerPacientePorId(int codigo)
        {
            PacienteModel paciente = null;
            var cn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(cn.getCadenaSQL()))
                {
                    conexion.Open();
                    using var cmd = new SqlCommand(
                        "SELECT Cedula, Nombre, Apellido FROM Paciente WHERE Codigo = @Codigo", conexion);
                    cmd.Parameters.AddWithValue("@Codigo", codigo);

                    using var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        paciente = new PacienteModel
                        {
                            Cedula = dr.GetString(0),
                            Nombre = dr.GetString(1),
                            Apellido = dr.GetString(2)
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en ObtenerPacientePorId: {ex.Message}");
            }

            return paciente;
        }

        // Obtener paciente completo (incluye ImagenPaciente)
        public PacienteModel ObtenerPacientePorIdCompleto(int codigo)
        {
            PacienteModel paciente = null;
            var cn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(cn.getCadenaSQL()))
                {
                    conexion.Open();
                    using var cmd = new SqlCommand(
                        @"SELECT Codigo, Cedula, Nombre, Apellido, Sexo, Edad, Ocupacion, EstadoCivil, Domicilio, Telefono, Estado, ImagenPaciente
                          FROM Paciente WHERE Codigo = @Codigo", conexion);
                    cmd.Parameters.AddWithValue("@Codigo", codigo);

                    using var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        paciente = new PacienteModel
                        {
                            Codigo = dr.GetInt32(0),
                            Cedula = dr["Cedula"]?.ToString(),
                            Nombre = dr["Nombre"]?.ToString(),
                            Apellido = dr["Apellido"]?.ToString(),
                            Sexo = dr["Sexo"]?.ToString(),
                            Edad = Convert.ToInt32(dr["Edad"]),
                            Ocupacion = dr["Ocupacion"]?.ToString(),
                            EstadoCivil = dr["EstadoCivil"]?.ToString(),
                            Domicilio = dr["Domicilio"]?.ToString(),
                            Telefono = dr["Telefono"]?.ToString(),
                            Estado = dr["Estado"]?.ToString(),
                            ImagenPaciente = dr["ImagenPaciente"] == DBNull.Value ? null : dr["ImagenPaciente"].ToString()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en ObtenerPacientePorIdCompleto: {ex.Message}");
            }

            return paciente;
        }

        // Guardar paciente (incluye @ImagenPaciente)
        public bool GuardarPaciente(PacienteModel paciente)
        {
            try
            {
                var cn = new Conexion();
                using (var conexion = new SqlConnection(cn.getCadenaSQL()))
                {
                    conexion.Open();
                    using var cmd = new SqlCommand("sp_GuardarPaciente", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Cedula", paciente.Cedula ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Nombre", paciente.Nombre ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Apellido", paciente.Apellido ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Sexo", paciente.Sexo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Edad", paciente.Edad);
                    cmd.Parameters.AddWithValue("@Ocupacion", paciente.Ocupacion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EstadoCivil", paciente.EstadoCivil ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Domicilio", paciente.Domicilio ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Telefono", paciente.Telefono ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", paciente.Estado ?? (object)DBNull.Value);

                    // Nuevo parámetro
                    var img = string.IsNullOrWhiteSpace(paciente.ImagenPaciente) ? (object)DBNull.Value : paciente.ImagenPaciente!;
                    cmd.Parameters.AddWithValue("@ImagenPaciente", img);

                    int filas = cmd.ExecuteNonQuery();
                    return filas > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en GuardarPaciente: {ex.Message}");
                return false;
            }
        }

        // Actualizar paciente (incluye @ImagenPaciente)
        public bool ActualizarPaciente(PacienteModel paciente)
        {
            try
            {
                var cn = new Conexion();
                using (var conexion = new SqlConnection(cn.getCadenaSQL()))
                {
                    conexion.Open();
                    using var cmd = new SqlCommand("sp_ActualizarPaciente", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Codigo", paciente.Codigo);
                    cmd.Parameters.AddWithValue("@Cedula", paciente.Cedula ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Nombre", paciente.Nombre ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Apellido", paciente.Apellido ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Sexo", paciente.Sexo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Edad", paciente.Edad);
                    cmd.Parameters.AddWithValue("@Ocupacion", paciente.Ocupacion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EstadoCivil", paciente.EstadoCivil ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Domicilio", paciente.Domicilio ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Telefono", paciente.Telefono ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", paciente.Estado ?? (object)DBNull.Value);

                    // Nuevo parámetro
                    var img = string.IsNullOrWhiteSpace(paciente.ImagenPaciente) ? (object)DBNull.Value : paciente.ImagenPaciente!;
                    cmd.Parameters.AddWithValue("@ImagenPaciente", img);

                    int filas = cmd.ExecuteNonQuery();
                    return filas > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en ActualizarPaciente: {ex.Message}");
                return false;
            }
        }

        // Eliminar paciente
        public bool EliminarPaciente(int codigo)
        {
            try
            {
                var cn = new Conexion();
                using (var conexion = new SqlConnection(cn.getCadenaSQL()))
                {
                    conexion.Open();
                    using var cmd = new SqlCommand("sp_EliminarPaciente", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Codigo", codigo);

                    int filas = cmd.ExecuteNonQuery();
                    return filas > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en EliminarPaciente: {ex.Message}");
                return false;
            }
        }
    }
}


