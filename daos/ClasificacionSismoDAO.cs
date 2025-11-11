using Microsoft.Data.SqlClient;
using PPAI_V2.entidades;
using PPAI_V2.gestor;
using System;
using System.Collections.Generic;

namespace PPAI_V2.daos
{
    public class ClasificacionSismoDAO
    {
        public List<ClasificacionSismo> ObtenerTodos()
        {
            var clasificaciones = new List<ClasificacionSismo>();
            SqlConnection conexion = null;

            try
            {
                conexion = ConexionDB.ObtenerConexion();
                string query = @"
                    SELECT Id, Nombre, ProfundidadMinKm, ProfundidadMaxKm 
                    FROM dbo.ClasificacionSismo 
                    ORDER BY ProfundidadMinKm";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var clasif = new ClasificacionSismo(
                            reader["Nombre"].ToString(),
                            Convert.ToDouble(reader["ProfundidadMinKm"]),
                            Convert.ToDouble(reader["ProfundidadMaxKm"])
                        );
                        typeof(ClasificacionSismo).GetProperty("Id")?.SetValue(clasif, Convert.ToInt32(reader["Id"]));
                        clasificaciones.Add(clasif);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener clasificaciones: {ex.Message}", ex);
            }
            finally
            {
                ConexionDB.CerrarConexion(conexion);
            }

            return clasificaciones;
        }
    }
}