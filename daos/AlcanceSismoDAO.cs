using Microsoft.Data.SqlClient;
using PPAI_V2.entidades;
using PPAI_V2.gestor;
using System;
using System.Collections.Generic;

namespace PPAI_V2.daos
{
    public class AlcanceSismoDAO
    {
        public List<AlcanceSismo> ObtenerTodos()
        {
            var alcances = new List<AlcanceSismo>();
            SqlConnection conexion = null;

            try
            {
                conexion = ConexionDB.ObtenerConexion();
                string query = "SELECT Id, Nombre, Descripcion FROM dbo.AlcanceSismo ORDER BY Nombre";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var alcance = new AlcanceSismo(
                            reader["Nombre"].ToString(),
                            reader["Descripcion"].ToString()
                        );
                        // Asignar Id si tu entidad lo permite
                        typeof(AlcanceSismo).GetProperty("Id")?.SetValue(alcance, Convert.ToInt32(reader["Id"]));
                        alcances.Add(alcance);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener alcances: {ex.Message}", ex);
            }
            finally
            {
                ConexionDB.CerrarConexion(conexion);
            }

            return alcances;
        }
    }
}