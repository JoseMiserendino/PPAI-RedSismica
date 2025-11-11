using Microsoft.Data.SqlClient;
using PPAI_V2.daos;
using PPAI_V2.entidades;
using System;
using System.Collections.Generic;

namespace PPAI_V2.gestor
{
    public class EventoSismicoDAO
    {
        private EstadoDAO estadoDAO = new EstadoDAO();
        private CambioEstadoDAO cambioEstadoDAO = new CambioEstadoDAO();
        private EmpleadoDAO empleadoDAO = new EmpleadoDAO();
        private SerieTemporalDAO serieTemporalDAO = new SerieTemporalDAO();

        public List<EventoSismico> ObtenerPorEstado(string nombreEstado)
        {
            List<EventoSismico> eventos = new List<EventoSismico>();
            SqlConnection conexion = null;
            try
            {
                conexion = ConexionDB.ObtenerConexion();
                string query = @"
            SELECT
                es.Id, es.FechaInicio, es.FechaFin,
                es.Latitud1, es.Longitud1, es.Latitud2, es.Longitud2,
                es.MagnitudValor,
                cs.Nombre AS Clasificacion,
                cs.ProfundidadMinKm, cs.ProfundidadMaxKm,
                mr.Nombre AS MagnitudNombre, mr.Valor AS MagnitudValorRichter,
                og.Nombre AS OrigenNombre, og.Descripcion AS OrigenDesc,
                alc.Nombre AS AlcanceNombre, alc.Descripcion AS AlcanceDesc,
                est.Nombre AS EstadoNombre
            FROM dbo.EventoSismico es
            JOIN dbo.ClasificacionSismo cs ON cs.Id = es.ClasificacionId
            JOIN dbo.MagnitudRichter mr ON mr.Id = es.MagnitudRichterId
            JOIN dbo.OrigenDeGeneracion og ON og.Id = es.OrigenId
            JOIN dbo.AlcanceSismo alc ON alc.Id = es.AlcanceId
            JOIN dbo.Estado est ON est.Id = es.EstadoActualId
            WHERE est.Nombre = @NombreEstado
            ORDER BY es.FechaInicio DESC";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@NombreEstado", nombreEstado);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EventoSismico evento = MapearEvento(reader);

                            // Ahora cargamos los cambios de estado por separado
                            int eventoId = Convert.ToInt32(reader["Id"]);
                            evento.CambiosEstado = cambioEstadoDAO.ObtenerPorEventoId(eventoId);
                            evento.serieTemporal = serieTemporalDAO.ObtenerPorEventoId(eventoId);
                            eventos.Add(evento);
                        }
                    }
                }
                return eventos;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener eventos: " + ex.Message);
            }
            finally
            {
                ConexionDB.CerrarConexion(conexion);
            }
        }

        public EventoSismico ObtenerPorId(int eventoId)
        {
            SqlConnection conexion = null;

            try
            {
                conexion = ConexionDB.ObtenerConexion();
                string query = @"
                    SELECT 
                        es.Id, es.FechaInicio, es.FechaFin,
                        es.Latitud1, es.Longitud1, es.Latitud2, es.Longitud2,
                        es.MagnitudValor,
                        cs.Nombre AS Clasificacion,
                        cs.ProfundidadMinKm, cs.ProfundidadMaxKm,
                        mr.Nombre AS MagnitudNombre, mr.Valor AS MagnitudValorRichter,
                        og.Nombre AS OrigenNombre, og.Descripcion AS OrigenDesc,
                        alc.Nombre AS AlcanceNombre, alc.Descripcion AS AlcanceDesc,
                        est.Nombre AS EstadoNombre
                    FROM dbo.EventoSismico es
                    JOIN dbo.ClasificacionSismo cs ON cs.Id = es.ClasificacionId
                    JOIN dbo.MagnitudRichter mr ON mr.Id = es.MagnitudRichterId
                    JOIN dbo.OrigenDeGeneracion og ON og.Id = es.OrigenId
                    JOIN dbo.AlcanceSismo alc ON alc.Id = es.AlcanceId
                    JOIN dbo.Estado est ON est.Id = es.EstadoActualId
                    WHERE es.Id = @EventoId";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@EventoId", eventoId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapearEvento(reader);
                        }
                    }
                }
                throw new Exception($"Evento con ID {eventoId} no encontrado");
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener evento: " + ex.Message);
            }
            finally
            {
                ConexionDB.CerrarConexion(conexion);
            }
        }

        private EventoSismico MapearEvento(SqlDataReader reader)
        {
            // Crear objetos relacionados
            ClasificacionSismo clasificacion = new ClasificacionSismo(
                reader["Clasificacion"].ToString(),
                Convert.ToDouble(reader["ProfundidadMinKm"]),
                Convert.ToDouble(reader["ProfundidadMaxKm"])
            );

            MagnitudRichter magnitud = new MagnitudRichter(
                reader["MagnitudNombre"].ToString(),
                Convert.ToDouble(reader["MagnitudValorRichter"])
            );

            OrigenDeGeneracion origen = new OrigenDeGeneracion(
                reader["OrigenNombre"].ToString(),
                reader["OrigenDesc"].ToString()
            );

            AlcanceSismo alcance = new AlcanceSismo(
                reader["AlcanceNombre"].ToString(),
                reader["AlcanceDesc"].ToString()
            );

            Estado estado = estadoDAO.ObtenerPorNombre(reader["EstadoNombre"].ToString());

            // Mapeo de columnas BD -> propiedades C#:
            // FechaInicio (BD) -> fechaHoraOcurrencia (C#)
            // FechaFin (BD) -> fechaHoraFin (C#)
            // Latitud1 (BD) -> latitudEpicentro (C#)
            // Longitud1 (BD) -> longitudEpicentro (C#)
            // Latitud2 (BD) -> latitudHipocentro (C#)
            // Longitud2 (BD) -> longitudHipocentro (C#)
            // MagnitudValor (BD) -> valorMagnitud (C#)

            EventoSismico evento = new EventoSismico(
            Convert.ToDateTime(reader["FechaInicio"]),
            Convert.ToDateTime(reader["FechaFin"]),
            Convert.ToDouble(reader["Latitud1"]),
            Convert.ToDouble(reader["Longitud1"]),
            Convert.ToDouble(reader["Latitud2"]),
            Convert.ToDouble(reader["Longitud2"]),
            Convert.ToDouble(reader["MagnitudValor"]),
            clasificacion,
            magnitud,
            origen,
            alcance,
            estado,
            new List<CambioEstado>(),
            new List<SerieTemporal>()
            );

            // IMPORTANTE: Asignar el ID después de crear el objeto
            evento.Id = Convert.ToInt32(reader["Id"]);

            return evento;
        }

        public int Insertar(EventoSismico evento)
        {
            SqlConnection conexion = null;

            try
            {
                conexion = ConexionDB.ObtenerConexion();

                // Primero necesitamos obtener los IDs de las entidades relacionadas
                int magnitudId = ObtenerIdMagnitudPorValor(evento.ValorMagnitud, conexion);
                int clasificacionId = ObtenerIdClasificacionPorNombre(evento.Clasificacion.Nombre, conexion);
                int origenId = ObtenerIdOrigenPorNombre(evento.OrigenGeneracion.Nombre, conexion);
                int alcanceId = ObtenerIdAlcancePorNombre(evento.AlcanceSismo.Nombre, conexion);
                int estadoId = ObtenerIdEstadoPorNombre("AutoDetectado", conexion); // Estado inicial

                string query = @"
                    INSERT INTO dbo.EventoSismico 
                    (FechaInicio, FechaFin, Latitud1, Longitud1, Latitud2, Longitud2,
                     MagnitudRichterId, ClasificacionId, OrigenId, AlcanceId, 
                     EstadoActualId, MagnitudValor)
                    VALUES 
                    (@FechaInicio, @FechaFin, @Lat1, @Lon1, @Lat2, @Lon2,
                     @MagId, @ClasId, @OrigenId, @AlcanceId, @EstadoId, @MagValor);
                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@FechaInicio", evento.FechaHoraOcurrencia);
                    cmd.Parameters.AddWithValue("@FechaFin", evento.FechaHoraFin);
                    cmd.Parameters.AddWithValue("@Lat1", evento.LatitudEpicentro);
                    cmd.Parameters.AddWithValue("@Lon1", evento.LongitudEpicentro);
                    cmd.Parameters.AddWithValue("@Lat2", evento.LatitudHipocentro);
                    cmd.Parameters.AddWithValue("@Lon2", evento.LongitudHipocentro);
                    cmd.Parameters.AddWithValue("@MagId", magnitudId);
                    cmd.Parameters.AddWithValue("@ClasId", clasificacionId);
                    cmd.Parameters.AddWithValue("@OrigenId", origenId);
                    cmd.Parameters.AddWithValue("@AlcanceId", alcanceId);
                    cmd.Parameters.AddWithValue("@EstadoId", estadoId);
                    cmd.Parameters.AddWithValue("@MagValor", evento.ValorMagnitud);

                    int nuevoId = Convert.ToInt32(cmd.ExecuteScalar());
                    return nuevoId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar evento: " + ex.Message);
            }
            finally
            {
                ConexionDB.CerrarConexion(conexion);
            }
        }


        public void Actualizar(EventoSismico evento)
        {
            SqlConnection conexion = null;
            try
            {
                conexion = ConexionDB.ObtenerConexion();

                // Obtener IDs de entidades relacionadas
                int alcanceId = ObtenerIdAlcancePorNombre(evento.AlcanceSismo.Nombre, conexion);
                int clasificacionId = ObtenerIdClasificacionPorNombre(evento.Clasificacion.Nombre, conexion);
                int origenId = ObtenerIdOrigenPorNombre(evento.OrigenGeneracion.Nombre, conexion);

                string query = @"
            UPDATE dbo.EventoSismico
            SET 
                AlcanceId = @AlcanceId,
                ClasificacionId = @ClasificacionId,
                OrigenId = @OrigenId
            WHERE Id = @EventoId";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@AlcanceId", alcanceId);
                    cmd.Parameters.AddWithValue("@ClasificacionId", clasificacionId);
                    cmd.Parameters.AddWithValue("@OrigenId", origenId);
                    cmd.Parameters.AddWithValue("@EventoId", evento.Id);

                    int filas = cmd.ExecuteNonQuery();
                    if (filas == 0)
                        throw new Exception($"No se encontró el evento ID {evento.Id}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar evento: {ex.Message}", ex);
            }
            finally
            {
                ConexionDB.CerrarConexion(conexion);
            }
        }
        // Métodos auxiliares para obtener IDs
        private int ObtenerIdMagnitudPorValor(double valor, SqlConnection conexion)
        {
            string query = "SELECT Id FROM dbo.MagnitudRichter WHERE Valor = @Valor";
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                cmd.Parameters.AddWithValue("@Valor", valor);
                object result = cmd.ExecuteScalar();
                if (result != null)
                    return Convert.ToInt32(result);
                throw new Exception($"Magnitud con valor {valor} no encontrada");
            }
        }

        private int ObtenerIdClasificacionPorNombre(string nombre, SqlConnection conexion)
        {
            string query = "SELECT Id FROM dbo.ClasificacionSismo WHERE Nombre = @Nombre";
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                object result = cmd.ExecuteScalar();
                if (result != null)
                    return Convert.ToInt32(result);
                throw new Exception($"Clasificación {nombre} no encontrada");
            }
        }

        private int ObtenerIdOrigenPorNombre(string nombre, SqlConnection conexion)
        {
            string query = "SELECT Id FROM dbo.OrigenDeGeneracion WHERE Nombre = @Nombre";
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                object result = cmd.ExecuteScalar();
                if (result != null)
                    return Convert.ToInt32(result);
                throw new Exception($"Origen {nombre} no encontrado");
            }
        }

        private int ObtenerIdAlcancePorNombre(string nombre, SqlConnection conexion)
        {
            string query = "SELECT Id FROM dbo.AlcanceSismo WHERE Nombre = @Nombre";
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                object result = cmd.ExecuteScalar();
                if (result != null)
                    return Convert.ToInt32(result);
                throw new Exception($"Alcance {nombre} no encontrado");
            }
        }

        private int ObtenerIdEstadoPorNombre(string nombre, SqlConnection conexion)
        {
            string query = "SELECT Id FROM dbo.Estado WHERE Nombre = @Nombre";
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                object result = cmd.ExecuteScalar();
                if (result != null)
                    return Convert.ToInt32(result);
                throw new Exception($"Estado {nombre} no encontrado");
            }
        }

        public void ActualizarEstado(int eventoId, string nombreEstado)
        {
            SqlConnection conexion = null;
            try
            {
                conexion = ConexionDB.ObtenerConexion();

                // Obtener el ID del nuevo estado
                int estadoId = ObtenerIdEstadoPorNombre(nombreEstado, conexion);

                string query = @"
            UPDATE dbo.EventoSismico 
            SET EstadoActualId = @EstadoId
            WHERE Id = @EventoId";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@EstadoId", estadoId);
                    cmd.Parameters.AddWithValue("@EventoId", eventoId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar estado del evento: " + ex.Message);
            }
            finally
            {
                ConexionDB.CerrarConexion(conexion);
            }
        }
    }
}