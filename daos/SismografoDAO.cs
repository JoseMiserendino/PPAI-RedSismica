using Microsoft.Data.SqlClient;
using PPAI_V2.entidades;
using PPAI_V2.gestor;
using System;
using System.Collections.Generic;

namespace PPAI_V2.daos
{
    public class SismografoDAO
    {
        private SerieTemporalDAO serieTemporalDAO = new SerieTemporalDAO();

        public List<Sismografo> ObtenerTodos()
        {
            List<Sismografo> sismografos = new List<Sismografo>();
            SqlConnection conexion = null;

            try
            {
                conexion = ConexionDB.ObtenerConexion();
                string query = @"
                    SELECT 
                        s.Id,
                        s.FechaInstalacion,
                        s.Codigo,
                        s.NumeroSerie,
                        s.EstacionCodigo,
                        es.Certificacion,
                        es.FechaCertificacion,
                        es.Latitud,
                        es.Longitud,
                        es.Ciudad,
                        es.IdInterno
                    FROM dbo.Sismografo s
                    JOIN dbo.EstacionSismologica es ON es.Codigo = s.EstacionCodigo
                    ORDER BY s.Codigo";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Sismografo sismografo = MapearSismografo(reader);
                        sismografos.Add(sismografo);
                    }
                }

                // Cargar las series temporales para cada sismógrafo
                foreach (var sismografo in sismografos)
                {
                    CargarSeriesTemporales(sismografo, conexion);
                }

                return sismografos;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener sismógrafos: " + ex.Message);
            }
            finally
            {
                ConexionDB.CerrarConexion(conexion);
            }
        }

        public Sismografo ObtenerPorId(int sismografoId)
        {
            SqlConnection conexion = null;

            try
            {
                conexion = ConexionDB.ObtenerConexion();
                string query = @"
                    SELECT 
                        s.Id,
                        s.FechaInstalacion,
                        s.Codigo,
                        s.NumeroSerie,
                        s.EstacionCodigo,
                        es.Certificacion,
                        es.FechaCertificacion,
                        es.Latitud,
                        es.Longitud,
                        es.Ciudad,
                        es.IdInterno
                    FROM dbo.Sismografo s
                    JOIN dbo.EstacionSismologica es ON es.Codigo = s.EstacionCodigo
                    WHERE s.Id = @SismografoId";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@SismografoId", sismografoId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Sismografo sismografo = MapearSismografo(reader);
                            CargarSeriesTemporales(sismografo, conexion);
                            return sismografo;
                        }
                    }
                }
                throw new Exception($"Sismógrafo con ID {sismografoId} no encontrado");
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener sismógrafo: " + ex.Message);
            }
            finally
            {
                ConexionDB.CerrarConexion(conexion);
            }
        }

        private Sismografo MapearSismografo(SqlDataReader reader)
        {
            // Crear estación sismológica
            EstacionSismologica estacion = new EstacionSismologica(
                reader["EstacionCodigo"].ToString(),
                reader["Certificacion"].ToString(),
                Convert.ToDateTime(reader["FechaCertificacion"]),
                Convert.ToDouble(reader["Latitud"]),
                Convert.ToDouble(reader["Longitud"]),
                reader["Ciudad"].ToString(),
                Convert.ToDouble(reader["IdInterno"])
            );

            // Crear sismógrafo (sin series temporales aún)
            Sismografo sismografo = new Sismografo(
                Convert.ToDateTime(reader["FechaInstalacion"]),
                reader["Codigo"].ToString(),
                reader["NumeroSerie"].ToString(),
                estacion,
                new List<SerieTemporal>() // Se cargarán después
            );

            return sismografo;
        }

        private void CargarSeriesTemporales(Sismografo sismografo, SqlConnection conexion)
        {
            try
            {
                // Obtener el ID del sismógrafo desde la BD
                int sismografoId = ObtenerIdSismografoPorCodigo(sismografo.IdentificadorSismografo, conexion);

                string query = @"
                    SELECT SerieId 
                    FROM dbo.SismografoSerie 
                    WHERE SismografoId = @SismografoId";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@SismografoId", sismografoId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<int> seriesIds = new List<int>();
                        while (reader.Read())
                        {
                            seriesIds.Add(Convert.ToInt32(reader["SerieId"]));
                        }

                        // Cargar cada serie temporal
                        foreach (int serieId in seriesIds)
                        {
                            SerieTemporal serie = serieTemporalDAO.ObtenerPorId(serieId);
                            sismografo.SerieTemporal.Add(serie);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar series temporales para sismógrafo {sismografo.IdentificadorSismografo}: {ex.Message}");
            }
        }

        private int ObtenerIdSismografoPorCodigo(string codigo, SqlConnection conexion)
        {
            string query = "SELECT Id FROM dbo.Sismografo WHERE Codigo = @Codigo";
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                cmd.Parameters.AddWithValue("@Codigo", codigo);
                object result = cmd.ExecuteScalar();
                if (result != null)
                    return Convert.ToInt32(result);
                throw new Exception($"Sismógrafo con código {codigo} no encontrado");
            }
        }
    }
}