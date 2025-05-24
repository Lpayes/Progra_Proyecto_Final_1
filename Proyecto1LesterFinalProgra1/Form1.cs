using Microsoft.Extensions.Configuration;
using System.IO;
using System.Globalization;
using System.Windows.Forms;
using Proyecto1LesterFinalProgra.Models;
using Proyecto1LesterFinalProgra.Config;
using Proyecto1LesterFinalProgra.Services;
using System.Text.RegularExpressions;

namespace Proyecto1LesterFinalProgra
{
    public partial class Form1 : Form
    {
        private readonly OpenAiService _openAiService;
        private readonly DocumentoService _documentoService;
        private readonly DatabaseService _databaseService;
        private string _tituloGenerado = "";
        private string _respuestaGenerada = "";
        private string _resumenGenerado = "";

        public Form1(DatabaseService databaseService)
        {
            InitializeComponent();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var settings = configuration.Get<AppSettings>();
            _openAiService = new OpenAiService(settings?.OpenAI?.ApiKey ?? throw new InvalidOperationException("API Key no configurada."));
            _documentoService = new DocumentoService();
            _databaseService = databaseService;
            txtPrompt.MaxLength = 255;
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtPrompt.Clear();
            txtRespuesta.Clear();
            txtPrompt.Focus();
        }

       private async void btnConsultar_Click(object sender, EventArgs e)
{
    if (string.IsNullOrEmpty(txtPrompt.Text.Trim()))
    {
        MessageBox.Show("Por favor ingresa un tema para consultar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
    }

    // Detecta si el usuario ingresó un prompt personalizado
    bool esPromptEstructurado = string.IsNullOrWhiteSpace(txtPromptPersonalizado.Text);

    // Permitir edición solo si es prompt libre
    txtRespuesta.ReadOnly = esPromptEstructurado;

    string promptFinal;
    if (esPromptEstructurado)
    {
        promptFinal = $@"Por favor, genera una respuesta detallada y académica en español sobre el tema: '{txtPrompt.Text.Trim()}'.
La respuesta debe estar dividida en las siguientes secciones, cada una comenzando con el título exacto seguido de dos puntos y en una línea aparte, sin formato Markdown ni símbolos adicionales:
Introducción:
Desarrollo:
Ejemplos:
Conclusiones:
Fuentes:
El formato debe ser profesional, con estructura clara y lenguaje técnico adecuado.";
    }
    else
    {
        promptFinal = txtPromptPersonalizado.Text.Trim();
    }

    btnConsultar.Enabled = false;
    txtRespuesta.Text = "Generando respuesta detallada... Por favor espere, esto puede tomar unos momentos.";

    try
    {
        var consultaParams = new OpenAIConsultaParams
        {
            Prompt = promptFinal,
            MaxTokens = 2000,
            Temperature = 0.7,
            TopP = 0.9,
            FrequencyPenalty = 0.5,
            PresencePenalty = 0.5
        };

        (string respuesta, int promptTokens, int totalTokens) = await _openAiService.HacerConsultaAsync(consultaParams);
        txtRespuesta.Text = respuesta;
        _respuestaGenerada = respuesta;

        string tituloPrompt = $"Genera un título académico profesional en español para un documento sobre: {txtPrompt.Text.Trim()}. El título debe ser claro, conciso y máximo 10 palabras.";
        _tituloGenerado = await _openAiService.GenerarTituloAsync(tituloPrompt);

        string resumenPrompt = $"Genera un resumen ejecutivo de 100 palabras en español para el siguiente contenido: {respuesta}";
        _resumenGenerado = await _openAiService.GenerarResumenAsync(resumenPrompt);

        MessageBox.Show("Consulta completada. Ahora puedes generar los documentos profesionales.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error al consultar OpenAI: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        txtRespuesta.Text = "";
    }
    finally
    {
        btnConsultar.Enabled = true;
    }
}

        private Dictionary<string, string> ExtraerSecciones(string texto)
        {
            var secciones = new Dictionary<string, string>
    {
        { "Introducción", "" },
        { "Desarrollo", "" },
        { "Ejemplos", "" },
        { "Conclusiones", "" },
        { "Fuentes", "" }
    };

            var regex = new Regex(
                @"^(#+\s*)?(?<titulo>Introducción|Desarrollo|Ejemplos|Conclusiones|Fuentes)\s*:?",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);

            var matches = regex.Matches(texto);

            for (int i = 0; i < matches.Count; i++)
            {
                var titulo = matches[i].Groups["titulo"].Value;
                int inicio = matches[i].Index + matches[i].Length;
                int fin = (i < matches.Count - 1) ? matches[i + 1].Index : texto.Length;
                string contenido = texto.Substring(inicio, fin - inicio).Trim();
                string clave = char.ToUpper(titulo[0]) + titulo.Substring(1).ToLower();
                if (secciones.ContainsKey(clave))
                    secciones[clave] = contenido;
            }

            return secciones;
        }

        private string GenerarDiapositivasPorSeccion(Dictionary<string, string> secciones)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var kvp in secciones)
            {
                if (!string.IsNullOrWhiteSpace(kvp.Value))
                {
                    sb.AppendLine($"{kvp.Key}");
                    sb.AppendLine(kvp.Value);
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }


        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_respuestaGenerada) || string.IsNullOrEmpty(_tituloGenerado))
            {
                MessageBox.Show("Primero realiza una consulta antes de guardar los archivos.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tituloLimpio = _documentoService.LimpiarNombreArchivo(_tituloGenerado);
            string carpetaDestino = ObtenerOCrearCarpetaEnEscritorio(tituloLimpio);

            bool esPromptEstructurado = string.IsNullOrWhiteSpace(txtPromptPersonalizado.Text);

            Dictionary<string, string> marcadores;
            string rutaPlantilla;
            string contenidoPowerPoint;

            if (esPromptEstructurado)
            {
                var secciones = ExtraerSecciones(_respuestaGenerada);
                marcadores = new Dictionary<string, string>
        {
            { "TITULO", LimpiarTexto(_tituloGenerado ?? "") },
            { "INTRODUCCION", LimpiarTexto(secciones.GetValueOrDefault("Introducción", "")) },
            { "DESARROLLO", LimpiarTexto(secciones.GetValueOrDefault("Desarrollo", "")) },
            { "EJEMPLOS", LimpiarTexto(secciones.GetValueOrDefault("Ejemplos", "")) },
            { "CONCLUSIONES", LimpiarTexto(secciones.GetValueOrDefault("Conclusiones", "")) },
            { "FUENTES", LimpiarTexto(secciones.GetValueOrDefault("Fuentes", "")) }
        };
                rutaPlantilla = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plantillas", "PlantillaWord.docx");
                contenidoPowerPoint = GenerarDiapositivasPorSeccion(_respuestaGenerada);
            }
            else
            {
                marcadores = new Dictionary<string, string>
        {
            { "TITULO", LimpiarTexto(_tituloGenerado ?? "") },
            { "LEYENDA", "Documento generado automáticamente" },
            { "FECHA", DateTime.Now.ToString("dd/MM/yyyy") },
            { "CUERPO", LimpiarTexto(_respuestaGenerada) }
        };
                rutaPlantilla = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plantillas", "PlantillaGenerica.docx");
                contenidoPowerPoint = $"Cuerpo\n{LimpiarTexto(_respuestaGenerada)}";
            }

            if (!File.Exists(rutaPlantilla))
            {
                MessageBox.Show($"No se encontró la plantilla en: {rutaPlantilla}");
                return;
            }

            btnGuardar.Enabled = false;
            try
            {
                await Task.Run(() =>
                {
                    _documentoService.CrearDocumentoWord(marcadores, tituloLimpio, rutaPlantilla, carpetaDestino);
                    _documentoService.CrearPresentacionPowerPoint(contenidoPowerPoint, _tituloGenerado, carpetaDestino);
                });

                string promptParaGuardar = esPromptEstructurado
            ? $"Tema: {txtPrompt.Text.Trim()}"
            : txtPromptPersonalizado.Text.Trim();

                await _databaseService.GuardarConsultaAsync(
                    promptParaGuardar,
                    _respuestaGenerada,
                    _tituloGenerado,
                    DateTime.Now
                );

                MessageBox.Show("Archivos generados con éxito en la carpeta del escritorio.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear los archivos: " + ex.Message);
            }
            finally
            {
                btnGuardar.Enabled = true;
            }
        }
        private string GenerarDiapositivasPorSeccion(string texto)
        {
            var secciones = new List<(string Titulo, List<string> Contenido)>();
            string tituloActual = null;
            var contenidoActual = new List<string>();

            var lineas = texto.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var l in lineas)
            {
                string linea = l.Replace("**", "").Trim();
                if (System.Text.RegularExpressions.Regex.IsMatch(linea, @"^\d+\.\s*"))
                    linea = System.Text.RegularExpressions.Regex.Replace(linea, @"^\d+\.\s*", "");

                if (EsEncabezadoDeSeccion(linea))
                {
                    if (tituloActual != null && contenidoActual.Count > 0)
                        secciones.Add((tituloActual, new List<string>(contenidoActual)));
                    tituloActual = QuitarDosPuntos(linea);
                    contenidoActual.Clear();
                }
                else
                {
                    contenidoActual.Add(linea);
                }
            }
            if (tituloActual != null && contenidoActual.Count > 0)
                secciones.Add((tituloActual, contenidoActual));

            var diapositivas = secciones.Select(s =>
                $"{s.Titulo}\n{string.Join(" ", s.Contenido)}"
            );
            return string.Join("\n\n", diapositivas);
        }

        private bool EsEncabezadoDeSeccion(string linea)
        {
            string[] posibles = { "Introducción", "Desarrollo", "Ejemplos", "Ejemplo", "Caso", "Conclusión", "Conclusiones", "Recomendaciones", "Fuentes", "Referencias" };
            string l = QuitarDosPuntos(linea).ToLowerInvariant();
            return posibles.Any(p => l.StartsWith(p.ToLowerInvariant())) && linea.Length < 40;
        }

        private string QuitarDosPuntos(string texto)
        {
            return texto.EndsWith(":") ? texto.Substring(0, texto.Length - 1).Trim() : texto.Trim();
        }
        private string ObtenerOCrearCarpetaEnEscritorio(string nombreCarpeta)
        {
            string escritorio = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string rutaCarpeta = Path.Combine(escritorio, nombreCarpeta);
            if (!Directory.Exists(rutaCarpeta))
                Directory.CreateDirectory(rutaCarpeta);
            return rutaCarpeta;
        }

        private string LimpiarSaltosDeLinea(string texto)
        {
            return System.Text.RegularExpressions.Regex.Replace(texto, @"(\r?\n){2,}", "\r\n").Trim();
        }

        private string LimpiarAsteriscos(string texto)
        {
            return texto.Replace("*", "");
        }
        private string LimpiarTexto(string texto)
        {
            return LimpiarAsteriscos(LimpiarSaltosDeLinea(texto));
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}