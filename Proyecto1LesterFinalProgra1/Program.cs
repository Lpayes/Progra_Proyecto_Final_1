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

            // Cargar configuraci�n desde appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            // Obtener la cadena de conexi�n
            string connectionString = configuration.GetConnectionString("SqlConnection");
            var dbService = new DatabaseService(connectionString);

            // Probar la conexi�n antes de abrir el formulario principal
            if (!dbService.TestConnection())
            {
                MessageBox.Show("No se pudo conectar a la base de datos. La aplicaci�n se cerrar�.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.Run(new Form1(dbService));
        }
    }
}