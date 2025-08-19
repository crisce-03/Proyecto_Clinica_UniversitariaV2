using Proyecto_Clinica_Universitaria.Models;
using System.Data.SqlClient;
using System.Data;

namespace Proyecto_Clinica_Universitaria.Datos
{
    public class MedicosEspecialidadDatos
    {
        public List<MedicosEspecialidadModel> Listar()
        {
            var lista = new List<MedicosEspecialidadModel>();
            var cn = new Conexion();

            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();

                // Usar el procedimiento almacenado sp_ListarEspecialidades
                SqlCommand cmd = new SqlCommand("sp_ListarEspecialidades", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new MedicosEspecialidadModel
                        {
                            Codigo = Convert.ToInt32(dr["Codigo"]),
                            Especialidad = dr["Especialidad"].ToString(),
                            Descripcion = dr["Descripcion"].ToString()
                        });
                    }
                }
            }

            return lista;
        }

        public MedicosEspecialidadModel Obtener(int Codigo)
        {
            var oCodigo = new MedicosEspecialidadModel();
            var cn = new Conexion();

            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("sp_ObtenerMedicosEspecialidad", conexion);
                cmd.Parameters.AddWithValue("Codigo", Codigo);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        oCodigo.Codigo = Convert.ToInt32(dr["Codigo"]);
                        oCodigo.Especialidad = dr["Especialidad"].ToString();
                        oCodigo.Descripcion = dr["Descripcion"].ToString();
                    }
                }
            }

            return oCodigo;
        }

        public bool Guardar(MedicosEspecialidadModel especialidadmedico)
        {
            bool result;
            try
            {
                var cn = new Conexion();
                using (var conexion = new SqlConnection(cn.getCadenaSQL()))
                {
                    conexion.Open();

                    SqlCommand cmd = new SqlCommand("sp_GuardarMedicosEspecialidad", conexion);
                    cmd.Parameters.AddWithValue("@Especialidad", especialidadmedico.Especialidad ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Descripcion", especialidadmedico.Descripcion ?? (object)DBNull.Value);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery();
                }

                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool Editar(MedicosEspecialidadModel especialidadmedico)
        {
            bool result;
            try
            {
                var cn = new Conexion();
                using (var conexion = new SqlConnection(cn.getCadenaSQL()))
                {
                    conexion.Open();

                    SqlCommand cmd = new SqlCommand("sp_EditarMedicosEspecialidad", conexion);
                    cmd.Parameters.AddWithValue("@Codigo", especialidadmedico.Codigo);
                    cmd.Parameters.AddWithValue("@Especialidad", especialidadmedico.Especialidad ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Descripcion", especialidadmedico.Descripcion ?? (object)DBNull.Value);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery();
                }

                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
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

                    SqlCommand cmd = new SqlCommand("sp_EliminarMedicosEspecialidad", conexion);
                    cmd.Parameters.AddWithValue("@Codigo", Codigo);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery();
                }

                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }
}