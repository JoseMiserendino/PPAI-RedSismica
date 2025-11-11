using Microsoft.Data.SqlClient;
using PPAI_V2.entidades;
using PPAI_V2.gestor;
using System;

namespace PPAI_V2.daos
{
    public class EmpleadoDAO
    {
        public Empleado ObtenerPorId(int empleadoId)
        {
            SqlConnection conexion = null;
            try
            {
                conexion = ConexionDB.ObtenerConexion();
                string query = "SELECT Id, Apellido, Nombre, Email, Telefono FROM dbo.Empleado WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Id", empleadoId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Empleado empleado = new Empleado(
                                Convert.ToInt32(reader["Id"]),
                                reader["Apellido"].ToString(),
                                reader["Nombre"].ToString(),
                                reader["Email"].ToString(),
                                reader["Telefono"].ToString()
                            );
                            return empleado;
                        }
                    }
                }
                throw new Exception($"Empleado con ID {empleadoId} no encontrado");
            }
            finally
            {
                ConexionDB.CerrarConexion(conexion);
            }
        }

        public Empleado ObtenerPorEmail(string email)
        {
            SqlConnection conexion = null;
            try
            {
                conexion = ConexionDB.ObtenerConexion();
                string query = @"
                    SELECT Id, Apellido, Nombre, Email, Telefono 
                    FROM dbo.Empleado 
                    WHERE Email = @Email";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Empleado(
                                Convert.ToInt32(reader["Id"]),
                                reader["Apellido"].ToString(),
                                reader["Nombre"].ToString(),
                                reader["Email"].ToString(),
                                reader["Telefono"].ToString()
                            );
                        }
                    }
                }

                // Si no lo encuentra
                throw new Exception($"Empleado con email '{email}' no encontrado.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar empleado por email: {ex.Message}", ex);
            }
            finally
            {
                ConexionDB.CerrarConexion(conexion);
            }
        }
    }
}