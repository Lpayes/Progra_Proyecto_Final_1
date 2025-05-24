using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto1LesterFinalProgra.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        /// <summary>
        /// Prueba la conexión a la base de datos.
        /// </summary>
        public bool TestConnection()
        {
            try
            {
                using var connection = GetConnection();
                connection.Open();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error de conexión: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public async Task GuardarConsultaAsync(string prompt, string resultado, string titulo, DateTime fecha)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            var command = new SqlCommand(
                @"INSERT INTO Consultas (Prompt, Resultado, Titulo, Fecha)
                  VALUES (@Prompt, @Resultado, @Titulo, @Fecha)", connection);

            command.Parameters.AddWithValue("@Prompt", prompt);
            command.Parameters.AddWithValue("@Resultado", resultado);
            command.Parameters.AddWithValue("@Titulo", titulo ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Fecha", fecha);

            await command.ExecuteNonQueryAsync();
        }
    }
}