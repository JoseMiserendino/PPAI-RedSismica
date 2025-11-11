using Microsoft.Data.SqlClient;
using PPAI_V2.entidades;
using PPAI_V2.gestor;
using System;
using System.Collections.Generic;

namespace PPAI_V2.daos
{
    public class CambioEstadoDAO
    {
        private EstadoDAO estadoDAO = new EstadoDAO();
        private EmpleadoDAO empleadoDAO = new EmpleadoDAO();

        public List<CambioEstado> ObtenerPorEventoId(int eventoId)
        {
            List<CambioEstado> cambios = new List<CambioEstado>();
            SqlConnection conexion = null;
            try
            {
                conexion = ConexionDB.ObtenerConexion();
                string query = @"
                    SELECT 
                        ce.FechaHora,
                        ce.FechaFin,
                        ce.EstadoId,
                        ce.EmpleadoId,
                        est.Nombre AS NombreEstado
                    FROM dbo.CambioEstado ce
                    JOIN dbo.Estado est ON est.Id = ce.EstadoId
                    WHERE ce.EventoId = @EventoId
                    ORDER BY ce.FechaHora";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@EventoId", eventoId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // === ÍNDICES UNA VEZ (soluciona IsDBNull(string)) ===
                        int idxEmpleadoId = reader.GetOrdinal("EmpleadoId");
                        int idxFechaHoraFin = reader.GetOrdinal("FechaFin");
                        int idxNombreEstado = reader.GetOrdinal("NombreEstado");
                        int idxFechaHora = reader.GetOrdinal("FechaHora");

                        while (reader.Read())
                        {
                            // Estado
                            string nombreEstado = reader.GetString(idxNombreEstado);
                            Estado estado = estadoDAO.ObtenerPorNombre(nombreEstado);

                            // Empleado (puede ser NULL)
                            Empleado empleado = null;
                            if (!reader.IsDBNull(idxEmpleadoId))
                            {
                                int empleadoId = reader.GetInt32(idxEmpleadoId);
                                empleado = empleadoDAO.ObtenerPorId(empleadoId);
                            }

                            // Fechas
                            DateTime fechaInicio = reader.GetDateTime(idxFechaHora);

                            // === SOLUCIÓN AL ERROR DE NULLABLE EN C# 7.3 ===
                            DateTime? fechaFin = null;
                            if (!reader.IsDBNull(idxFechaHoraFin))
                            {
                                fechaFin = reader.GetDateTime(idxFechaHoraFin);
                            }

                            // === CREAR CAMBIO DE ESTADO USANDO EL CONSTRUCTOR CORRECTO ===
                            CambioEstado cambio;
                            if (fechaFin.HasValue)
                            {
                                cambio = new CambioEstado(fechaInicio, fechaFin.Value, estado, empleado);
                            }
                            else
                            {
                                cambio = new CambioEstado(fechaInicio, estado, empleado);
                            }

                            cambios.Add(cambio);
                        }
                    }
                }
                return cambios;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener cambios de estado: " + ex.Message);
            }
            finally
            {
                ConexionDB.CerrarConexion(conexion);
            }
        }

        // daos/CambioEstadoDAO.cs (si no lo tenés, crealo)
        // En CambioEstadoDAO.cs - CORREGIR el método Insertar
        public int Insertar(CambioEstado cambio, int eventoId, int? empleadoId)
        {
            SqlConnection conexion = null;
            try
            {
                conexion = ConexionDB.ObtenerConexion();

                // Obtener el ID del estado
                int estadoId = ObtenerIdEstadoPorNombre(cambio.Estado.NombreEstado, conexion);

                // NO insertar FechaFin - debe quedar NULL para el cambio actual
                string query = @"
            INSERT INTO dbo.CambioEstado (EventoId, FechaHora, EstadoId, EmpleadoId)
            VALUES (@EventoId, @FechaHora, @EstadoId, @EmpleadoId);
            SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@EventoId", eventoId);
                    cmd.Parameters.AddWithValue("@FechaHora", cambio.FechaHoraInicio);
                    cmd.Parameters.AddWithValue("@EstadoId", estadoId);
                    cmd.Parameters.AddWithValue("@EmpleadoId", empleadoId.HasValue ? (object)empleadoId.Value : DBNull.Value);

                    int nuevoId = Convert.ToInt32(cmd.ExecuteScalar());
                    Console.WriteLine($"Nuevo CambioEstado insertado con ID: {nuevoId}");
                    return nuevoId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar cambio de estado: " + ex.Message);
            }
            finally
            {
                ConexionDB.CerrarConexion(conexion);
            }
        }

        // Método auxiliar para obtener ID del estado
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

        public void ActualizarFechaFinUltimoCambio(int eventoId, DateTime fechaFin)
        {
            SqlConnection conexion = null;
            try
            {
                conexion = ConexionDB.ObtenerConexion();

                // Actualizar el último cambio de estado (el que NO tiene FechaFin)
                string query = @"
            UPDATE dbo.CambioEstado 
            SET FechaFin = @FechaFin
            WHERE EventoId = @EventoId 
            AND FechaFin IS NULL";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                    cmd.Parameters.AddWithValue("@EventoId", eventoId);

                    int filasAfectadas = cmd.ExecuteNonQuery();
                    Console.WriteLine($"Filas actualizadas en CambioEstado: {filasAfectadas}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar fecha fin del cambio de estado: " + ex.Message);
            }
            finally
            {
                ConexionDB.CerrarConexion(conexion);
            }
        }
    }
}