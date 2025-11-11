using Microsoft.Data.SqlClient;
using PPAI_V2.entidades;
using PPAI_V2.gestor;
using System;
using System.Collections.Generic;

namespace PPAI_V2.daos
{
    public class SesionDAO
    {
        //public Estado ObtenerPorNombre(string nombreEstado)
        //{
        //    SqlConnection conexion = null;
        //    try
        //    {
        //        conexion = ConexionDB.ObtenerConexion();
        //        string query = "SELECT Nombre FROM dbo.Estado WHERE Nombre = @Nombre";

        //        using (SqlCommand cmd = new SqlCommand(query, conexion))
        //        {
        //            cmd.Parameters.AddWithValue("@Nombre", nombreEstado);
        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                if (reader.Read())
        //                {
        //                    return CrearEstado(reader["Nombre"].ToString());
        //                }
        //            }
        //        }
        //        throw new Exception($"Estado '{nombreEstado}' no encontrado");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error al obtener estado: " + ex.Message);
        //    }
        //    finally
        //    {
        //        ConexionDB.CerrarConexion(conexion);
        //    }
        //}
    }
}
