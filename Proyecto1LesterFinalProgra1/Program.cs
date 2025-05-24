using Microsoft.Extensions.Configuration;
using Proyecto1LesterFinalProgra.Services;
using System.Windows.Forms;

namespace Proyecto1LesterFinalProgra
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Cargar configuración desde appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            // Obtener la cadena de conexión
            string connectionString = configuration.GetConnectionString("SqlConnection");
            var dbService = new DatabaseService(connectionString);

            // Probar la conexión antes de abrir el formulario principal
            if (!dbService.TestConnection())
            {
                MessageBox.Show("No se pudo conectar a la base de datos. La aplicación se cerrará.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.Run(new Form1(dbService));
        }
    }
}