using Microsoft.Data.SqlClient;
using PPAI_V2.entidades;
using PPAI_V2.gestor;
using System;
using System.Collections.Generic;

namespace PPAI_V2.daos
{
    public class EstadoDAO
    {
        public Estado ObtenerPorNombre(string nombreEstado)
        {
            SqlConnection conexion = null;
            try
            {
                conexion = ConexionDB.ObtenerConexion();
                string query = "SELECT Nombre FROM dbo.Estado WHERE Nombre = @Nombre";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombreEstado);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return CrearEstado(reader["Nombre"].ToString());
                        }
                    }
                }
                throw new Exception($"Estado '{nombreEstado}' no encontrado");
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener estado: " + ex.Message);
            }
            finally
            {
                ConexionDB.CerrarConexion(conexion);
            }
        }

        public List<Estado> ObtenerTodos()
        {
            List<Estado> estados = new List<Estado>();
            SqlConnection conexion = null;

            try
            {
                conexion = ConexionDB.ObtenerConexion();
                string query = "SELECT Nombre FROM dbo.Estado ORDER BY Nombre";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Estado estado = CrearEstado(reader["Nombre"].ToString());
                        estados.Add(estado);
                    }
                }
                return estados;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener estados: " + ex.Message);
            }
            finally
            {
                ConexionDB.CerrarConexion(conexion);
            }
        }

        private Estado CrearEstado(string nombreEstado)
        {
            switch (nombreEstado)
            {
                case "AutoConfirmado":
                    return new AutoConfirmado();
                case "AutoDetectado":
                    return new AutoDetectado();
                case "BloqueadoEnRevision":
                    return new BloqueadoEnRevision();
                case "Rechazado":
                    return new Rechazado();
                default:
                    throw new Exception($"Estado desconocido: {nombreEstado}");
            }
        }
    }
}