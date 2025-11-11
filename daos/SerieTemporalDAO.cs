using Microsoft.Data.SqlClient;
using PPAI_V2.entidades;
using PPAI_V2.gestor;
using System;
using System.Collections.Generic;

namespace PPAI_V2.daos
{
    public class SerieTemporalDAO
    {
        
        private CacheTipoDeDato cacheTipoDeDato = new CacheTipoDeDato();

        public SerieTemporal ObtenerPorId(int serieId)
        {
            SqlConnection conexion = null;
            try
            {
                conexion = ConexionDB.ObtenerConexion();
                string query = @"
            SELECT
                st.Id AS SerieId,
                st.CondicionAlarma,
                st.FechaHoraInicio,
                st.FechaHoraFin,
                st.FrecuenciaMuestreoHz,
               
                ms.Id AS MuestraId,
                ms.FechaHora,
               
                dms.Valor,
               
                td.Nombre,
                td.Unidad,
                td.ValorReferencia
            FROM dbo.SerieTemporal st
            LEFT JOIN dbo.MuestraSismica ms ON ms.SerieId = st.Id
            LEFT JOIN dbo.DetalleMuestraSismica dms ON dms.MuestraId = ms.Id
            LEFT JOIN dbo.TipoDeDato td ON td.Id = dms.TipoDatoId
            WHERE st.Id = @SerieId
            ORDER BY ms.FechaHora";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@SerieId", serieId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // === OBTENER ÍNDICES UNA SOLA VEZ ===
                        int idxSerieId = reader.GetOrdinal("SerieId");
                        int idxCondicionAlarma = reader.GetOrdinal("CondicionAlarma");
                        int idxFechaInicio = reader.GetOrdinal("FechaHoraInicio");
                        int idxFechaFin = reader.GetOrdinal("FechaHoraFin");
                        int idxFrecuencia = reader.GetOrdinal("FrecuenciaMuestreoHz");
                        int idxMuestraId = reader.GetOrdinal("MuestraId");
                        int idxFechaMuestra = reader.GetOrdinal("FechaHora");
                        int idxValor = reader.GetOrdinal("Valor");
                        int idxNombre = reader.GetOrdinal("Nombre");
                        int idxUnidad = reader.GetOrdinal("Unidad");
                        int idxValorRef = reader.GetOrdinal("ValorReferencia");

                        SerieTemporal serieActual = null;
                        MuestraSismica muestraActual = null;
                        int ultimaMuestraId = -1;

                        while (reader.Read())
                        {
                            // === CREAR SERIE (solo la primera vez) ===
                            if (serieActual == null)
                            {
                                double condicion = (double)reader.GetDecimal(idxCondicionAlarma);
                                DateTime inicio = reader.GetDateTime(idxFechaInicio);
                                DateTime fin = reader.GetDateTime(idxFechaFin);
                                double frecuencia = (double)reader.GetDecimal(idxFrecuencia);

                                serieActual = new SerieTemporal(
                                    condicion,
                                    inicio,
                                    fin,
                                    frecuencia,
                                    new List<MuestraSismica>()
                                );
                            }

                            // === PROCESAR MUESTRAS ===
                            if (!reader.IsDBNull(idxMuestraId))
                            {
                                int muestraId = reader.GetInt32(idxMuestraId);

                                if (muestraId != ultimaMuestraId)
                                {
                                    DateTime fechaMuestra = reader.GetDateTime(idxFechaMuestra);
                                    muestraActual = new MuestraSismica(
                                        fechaMuestra,
                                        new List<DetalleMuestraSismica>()
                                    );
                                    serieActual.MuestrasSismicas.Add(muestraActual);
                                    ultimaMuestraId = muestraId;
                                }

                                double valor = (double)reader.GetDecimal(idxValor);
                                string nombre = reader.GetString(idxNombre);
                                string unidad = reader.IsDBNull(idxUnidad) ? "" : reader.GetString(idxUnidad);
                                double valorRef = reader.IsDBNull(idxValorRef) ? 0.0 : (double)reader.GetDecimal(idxValorRef);

                                TipoDeDato tipoDato = cacheTipoDeDato.ObtenerOCrear(nombre, unidad, valorRef);
                                DetalleMuestraSismica detalle = new DetalleMuestraSismica(valor, tipoDato);
                                muestraActual.detalleMuestraSismica.Add(detalle);
                            }
                        }

                        if (serieActual == null)
                            throw new Exception($"SerieTemporal con Id {serieId} no encontrada.");

                        return serieActual;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"ERROR al obtener SerieTemporal por Id: {ex.Message}", ex);
            }
            finally
            {
                ConexionDB.CerrarConexion(conexion);
            }
        }

        public List<SerieTemporal> ObtenerPorEventoId(int eventoId)
        {
            var seriesDict = new Dictionary<int, SerieTemporal>();
            SqlConnection conexion = null;

            try
            {
                conexion = ConexionDB.ObtenerConexion();
                string query = @"
                    SELECT 
                        st.Id AS SerieId,
                        st.CondicionAlarma,
                        st.FechaHoraInicio,
                        st.FechaHoraFin,
                        st.FrecuenciaMuestreoHz,
                        
                        ms.Id AS MuestraId,
                        ms.FechaHora,
                        
                        dms.Valor,
                        
                        td.Nombre,
                        td.Unidad,
                        td.ValorReferencia
                    FROM dbo.SerieTemporal st
                    LEFT JOIN dbo.MuestraSismica ms ON ms.SerieId = st.Id
                    LEFT JOIN dbo.DetalleMuestraSismica dms ON dms.MuestraId = ms.Id
                    LEFT JOIN dbo.TipoDeDato td ON td.Id = dms.TipoDatoId
                    WHERE st.EventoId = @EventoId
                    ORDER BY st.Id, ms.FechaHora";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@EventoId", eventoId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int idxSerieId = reader.GetOrdinal("SerieId");
                        int idxCondicionAlarma = reader.GetOrdinal("CondicionAlarma");
                        int idxFechaInicio = reader.GetOrdinal("FechaHoraInicio");
                        int idxFechaFin = reader.GetOrdinal("FechaHoraFin");
                        int idxFrecuencia = reader.GetOrdinal("FrecuenciaMuestreoHz");

                        int idxMuestraId = reader.GetOrdinal("MuestraId");
                        int idxFechaMuestra = reader.GetOrdinal("FechaHora");
                        int idxValor = reader.GetOrdinal("Valor");
                        int idxNombre = reader.GetOrdinal("Nombre");
                        int idxUnidad = reader.GetOrdinal("Unidad");
                        int idxValorRef = reader.GetOrdinal("ValorReferencia");

                        int ultimoSerieId = -1;
                        int ultimaMuestraId = -1;
                        SerieTemporal serieActual = null;
                        MuestraSismica muestraActual = null;

                        while (reader.Read())
                        {
                            int serieId = reader.GetInt32(idxSerieId);

                            if (serieId != ultimoSerieId)
                            {
                                double condicion = (double)reader.GetDecimal(idxCondicionAlarma);
                                DateTime inicio = reader.GetDateTime(idxFechaInicio);
                                DateTime fin = reader.GetDateTime(idxFechaFin);
                                double frecuencia = (double)reader.GetDecimal(idxFrecuencia);

                                serieActual = new SerieTemporal(
                                    condicion,
                                    inicio,
                                    fin,
                                    frecuencia,
                                    new List<MuestraSismica>()
                                );
                                seriesDict[serieId] = serieActual;
                                ultimoSerieId = serieId;
                                ultimaMuestraId = -1;
                            }

                            if (!reader.IsDBNull(idxMuestraId))
                            {
                                int muestraId = reader.GetInt32(idxMuestraId);
                                if (muestraId != ultimaMuestraId)
                                {
                                    DateTime fechaMuestra = reader.GetDateTime(idxFechaMuestra);
                                    muestraActual = new MuestraSismica(
                                        fechaMuestra,
                                        new List<DetalleMuestraSismica>()
                                    );
                                    serieActual.MuestrasSismicas.Add(muestraActual);
                                    ultimaMuestraId = muestraId;
                                }

                                double valor = (double)reader.GetDecimal(idxValor);
                                string nombre = reader.GetString(idxNombre);
                                string unidad = reader.IsDBNull(idxUnidad) ? "" : reader.GetString(idxUnidad);
                                double valorRef = reader.IsDBNull(idxValorRef) ? 0.0 : (double)reader.GetDecimal(idxValorRef);

                                TipoDeDato tipoDato = cacheTipoDeDato.ObtenerOCrear(nombre, unidad, valorRef);
                                DetalleMuestraSismica detalle = new DetalleMuestraSismica(valor, tipoDato);
                                muestraActual.detalleMuestraSismica.Add(detalle);
                            }
                        }
                    }
                }

                return new List<SerieTemporal>(seriesDict.Values);
            }
            catch (Exception ex)
            {
                throw new Exception($"ERROR SerieTemporalDAO: {ex.Message}");
            }
            finally
            {
                ConexionDB.CerrarConexion(conexion);
            }
        }
    }

    // <<< NOMBRE CAMBIADO PARA EVITAR AMBIGÜEDAD >>>
    internal class CacheTipoDeDato
    {
        private Dictionary<string, TipoDeDato> cache = new Dictionary<string, TipoDeDato>();

        public TipoDeDato ObtenerOCrear(string denominacion, string unidad, double umbral)
        {
            string clave = $"{denominacion}|{unidad}";
            if (!cache.TryGetValue(clave, out TipoDeDato tipo))
            {
                tipo = new TipoDeDato(denominacion, unidad, umbral);
                cache[clave] = tipo;
            }
            return tipo;
        }
    }
}