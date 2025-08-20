using Proyecto_Clinica_Universitaria.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Proyecto_Clinica_Universitaria.Datos
{
    public class ConsultasDatos
    {
        public List<ConsultaVistaModel> ListarConsultasConMedico()
        {
            var lista = new List<ConsultaVistaModel>();
            var cn = new Conexion();

            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();

                var sql = @"
            SELECT c.Codigo, c.Fecha, c.Cedula, c.Nombre, c.Apellido, c.Temperatura, c.Pulso, c.Peso, 
                   c.Presion, c.SwatO2, m.Nombre AS MedicoNombre, med.Medicamento AS MedicamentoNombre
            FROM Consulta c
            INNER JOIN Medico m ON c.Medico = m.Codigo
            INNER JOIN Medicamentos med ON c.Medicamento = med.Codigo
            ORDER BY c.Fecha DESC";

                SqlCommand cmd = new SqlCommand(sql, conexion);

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ConsultaVistaModel
                        {
                            Codigo = Convert.ToInt32(dr["Codigo"]),
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            Cedula = dr["Cedula"].ToString(),
                            Nombre = dr["Nombre"].ToString(),
                            Apellido = dr["Apellido"].ToString(),
                            Temperatura = dr["Temperatura"].ToString(),
                            Pulso = dr["Pulso"].ToString(),
                            Peso = dr["Peso"].ToString(),
                            Presion = dr["Presion"].ToString(),
                            SwatO2 = dr["SwatO2"].ToString(),
                            MedicoNombre = dr["MedicoNombre"].ToString(),
                            MedicamentoNombre = dr["MedicamentoNombre"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        // Nuevo método para obtener una consulta por código
        public ConsultasModel ObtenerPorCodigo(int codigo)
        {
            ConsultasModel consulta = null;
            var cn = new Conexion();

            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();

                var sql = @"
                    SELECT Codigo, Paciente, Cedula, Nombre, Apellido, Fecha, Temperatura, Pulso, Peso, Presion, SwatO2,
                           Recordatorio, Evolucion, Prescripcion, Medico, Medicamento
                    FROM Consulta
                    WHERE Codigo = @Codigo";

                SqlCommand cmd = new SqlCommand(sql, conexion);
                cmd.Parameters.AddWithValue("@Codigo", codigo);

                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        consulta = new ConsultasModel
                        {
                            Codigo = Convert.ToInt32(dr["Codigo"]),
                            Paciente = Convert.ToInt32(dr["Paciente"]),
                            Cedula = dr["Cedula"] == DBNull.Value ? null : dr["Cedula"].ToString(),
                            Nombre = dr["Nombre"] == DBNull.Value ? null : dr["Nombre"].ToString(),
                            Apellido = dr["Apellido"] == DBNull.Value ? null : dr["Apellido"].ToString(),
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            Temperatura = dr["Temperatura"] == DBNull.Value ? null : dr["Temperatura"].ToString(),
                            Pulso = dr["Pulso"] == DBNull.Value ? null : dr["Pulso"].ToString(),
                            Peso = dr["Peso"] == DBNull.Value ? null : dr["Peso"].ToString(),
                            Presion = dr["Presion"] == DBNull.Value ? null : dr["Presion"].ToString(),
                            SwatO2 = dr["SwatO2"] == DBNull.Value ? null : dr["SwatO2"].ToString(),
                            Recordatorio = dr["Recordatorio"] == DBNull.Value ? null : dr["Recordatorio"].ToString(),
                            Evolucion = dr["Evolucion"] == DBNull.Value ? null : dr["Evolucion"].ToString(),
                            Prescripcion = dr["Prescripcion"] == DBNull.Value ? null : dr["Prescripcion"].ToString(),
                            Medico = Convert.ToInt32(dr["Medico"]),
                            Medicamento = Convert.ToInt32(dr["Medicamento"])
                        };
                    }
                }
            }
            return consulta;
        }

        public bool Guardar(ConsultasModel consultas)
        {
            try
            {
                var cn = new Conexion();
                using (var conexion = new SqlConnection(cn.getCadenaSQL()))
                {
                    conexion.Open();

                    using (var cmd = new SqlCommand("sp_GuardarConsulta", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Paciente", consultas.Paciente);
                        cmd.Parameters.AddWithValue("@Cedula", consultas.Cedula ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Nombre", consultas.Nombre ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Apellido", consultas.Apellido ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Fecha", consultas.Fecha);
                        cmd.Parameters.AddWithValue("@Temperatura", consultas.Temperatura ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Pulso", consultas.Pulso ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Peso", consultas.Peso ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Presion", consultas.Presion ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SwatO2", consultas.SwatO2 ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Recordatorio", consultas.Recordatorio ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Evolucion", consultas.Evolucion ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Prescripcion", consultas.Prescripcion ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Medico", consultas.Medico);
                        cmd.Parameters.AddWithValue("@Medicamento", consultas.Medicamento);

                        int filasAfectadas = cmd.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Guardar: {ex.Message}");
                throw;
            }
        }

        public bool Editar(ConsultasModel consultas)
        {
            try
            {
                var cn = new Conexion();
                using (var conexion = new SqlConnection(cn.getCadenaSQL()))
                {
                    conexion.Open();

                    using (var cmd = new SqlCommand("sp_EditarConsulta", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Codigo", consultas.Codigo);
                        cmd.Parameters.AddWithValue("@Paciente", consultas.Paciente);
                        cmd.Parameters.AddWithValue("@Cedula", consultas.Cedula ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Nombre", consultas.Nombre ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Apellido", consultas.Apellido ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Fecha", consultas.Fecha);
                        cmd.Parameters.AddWithValue("@Temperatura", consultas.Temperatura ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Pulso", consultas.Pulso ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Peso", consultas.Peso ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Presion", consultas.Presion ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SwatO2", consultas.SwatO2 ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Recordatorio", consultas.Recordatorio ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Evolucion", consultas.Evolucion ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Prescripcion", consultas.Prescripcion ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Medico", consultas.Medico);
                        cmd.Parameters.AddWithValue("@Medicamento", consultas.Medicamento);

                        int filasAfectadas = cmd.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Editar: {ex.Message}");
                throw;
            }
        }


        public bool Eliminar(int Codigo)
        {
            bool result;
            try
            {
                var cn = new Conexion();
                using (var conexion = new SqlConnection(cn.getCadenaSQL()))
                {
                    conexion.Open();

                    SqlCommand cmd = new SqlCommand("sp_EliminarConsulta", conexion);
                    cmd.Parameters.AddWithValue("@Codigo", Codigo);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery();
                }

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Eliminar: {ex.Message}");
                throw; ;
            }
            return result;
        }
    }
}

