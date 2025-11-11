using Microsoft.Data.SqlClient;
using PPAI_V2.entidades;
using PPAI_V2.gestor;
using System;
using System.Collections.Generic;

namespace PPAI_V2.daos
{
    public class OrigenDeGeneracionDAO
    {
        public List<OrigenDeGeneracion> ObtenerTodos()
        {
            var origenes = new List<OrigenDeGeneracion>();
            SqlConnection conexion = null;

            try
            {
                conexion = ConexionDB.ObtenerConexion();
                string query = "SELECT Id, Nombre, Descripcion FROM dbo.OrigenDeGeneracion ORDER BY Nombre";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var origen = new OrigenDeGeneracion(
                            reader["Nombre"].ToString(),
                            reader["Descripcion"].ToString()
                        );
                        typeof(OrigenDeGeneracion).GetProperty("Id")?.SetValue(origen, Convert.ToInt32(reader["Id"]));
                        origenes.Add(origen);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener orígenes: {ex.Message}", ex);
            }
            finally
            {
                ConexionDB.CerrarConexion(conexion);
            }

            return origenes;
        }
    }
}