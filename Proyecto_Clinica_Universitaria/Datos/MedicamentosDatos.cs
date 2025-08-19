using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Proyecto_Clinica_Universitaria.Models;

namespace Proyecto_Clinica_Universitaria.Datos
{
    public class MedicamentosDatos
    {
        public List<MedicamentoModel> Listar()
        {
            var lista = new List<MedicamentoModel>();
            var cn = new Conexion();

            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_ListarMedicamentos", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new MedicamentoModel
                            {
                                Codigo = Convert.ToInt32(dr["Codigo"]),
                                Medicamento = dr["Medicamento"].ToString() ?? string.Empty,
                                Descripcion = dr["Descripcion"] == DBNull.Value ? null : dr["Descripcion"]!.ToString(),
                                Cantidad = Convert.ToInt32(dr["Cantidad"]),
                                Vencimiento = dr["Vencimiento"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["Vencimiento"]),
                                Imagen = dr["Imagen"] == DBNull.Value ? null : dr["Imagen"]!.ToString()
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public MedicamentoModel? Obtener(int codigo)
        {
            MedicamentoModel? obj = null;
            var cn = new Conexion();

            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_ObtenerMedicamento", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Codigo", codigo);

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            obj = new MedicamentoModel
                            {
                                Codigo = Convert.ToInt32(dr["Codigo"]),
                                Medicamento = dr["Medicamento"].ToString() ?? string.Empty,
                                Descripcion = dr["Descripcion"] == DBNull.Value ? null : dr["Descripcion"]!.ToString(),
                                Cantidad = Convert.ToInt32(dr["Cantidad"]),
                                Vencimiento = dr["Vencimiento"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["Vencimiento"]),
                                Imagen = dr["Imagen"] == DBNull.Value ? null : dr["Imagen"]!.ToString()
                            };
                        }
                    }
                }
            }
            return obj;
        }

        public bool Guardar(MedicamentoModel obj)
        {
            var cn = new Conexion();
            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_GuardarMedicamentos", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Medicamento", obj.Medicamento);
                    cmd.Parameters.AddWithValue("@Descripcion", (object?)obj.Descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Cantidad", obj.Cantidad);
                    cmd.Parameters.AddWithValue("@Vencimiento", (object?)obj.Vencimiento ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Imagen", (object?)obj.Imagen ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
            return true;
        }

        public bool Editar(MedicamentoModel obj)
        {
            var cn = new Conexion();
            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_EditarMedicamentos", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Codigo", obj.Codigo);
                    cmd.Parameters.AddWithValue("@Medicamento", obj.Medicamento);
                    cmd.Parameters.AddWithValue("@Descripcion", (object?)obj.Descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Cantidad", obj.Cantidad);
                    cmd.Parameters.AddWithValue("@Vencimiento", (object?)obj.Vencimiento ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Imagen", (object?)obj.Imagen ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
            return true;
        }

        public bool Eliminar(int codigo)
        {
            var cn = new Conexion();
            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_EliminarMedicamento", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Codigo", codigo);
                    cmd.ExecuteNonQuery();
                }
            }
            return true;
        }
    }
}
