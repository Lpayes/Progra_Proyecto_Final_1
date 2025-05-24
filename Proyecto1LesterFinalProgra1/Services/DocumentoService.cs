using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WD = Microsoft.Office.Interop.Word;
using PP = Microsoft.Office.Interop.PowerPoint;
using Office = Microsoft.Office.Core;
using System.Text;

namespace Proyecto1LesterFinalProgra.Services
{
    public class DocumentoService
    {
        public void CrearDocumentoWord(Dictionary<string, string> marcadores, string titulo, string plantillaPath, string carpetaDestino)
        {
            var wordApp = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document doc = null;
            try
            {
                object missing = System.Reflection.Missing.Value;
                object readOnly = false;
                object isVisible = false;
                object templatePath = plantillaPath;

                doc = wordApp.Documents.Open(ref templatePath, ref missing, ref readOnly, ref missing, ref missing, ref missing,
                                             ref missing, ref missing, ref missing, ref missing, ref missing, ref isVisible, ref missing, ref missing, ref missing, ref missing);

                wordApp.Visible = false;
                doc.Activate();

                foreach (var kvp in marcadores)
                {
                    if (kvp.Value == null)
                        throw new ArgumentNullException($"El marcador {kvp.Key} tiene un valor null.");

                    string bookmarkName = kvp.Key;
                    if (doc.Bookmarks.Exists(bookmarkName))
                    {
                        doc.Bookmarks[bookmarkName].Range.Text = kvp.Value;
                    }
                }

                // Guarda en la carpeta destino
                string outputPath = Path.Combine(carpetaDestino, $"{LimpiarNombreArchivo(titulo)}.docx");
                doc.SaveAs2(outputPath);
                doc.Close();
            }
            finally
            {
                wordApp.Quit();
            }
        }
        public string LimpiarNombreArchivo(string nombre)
        {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
            return Regex.Replace(nombre, invalidRegStr, "_").Replace(" ", "_");
        }
        private List<string> DividirTextoPorPuntos(string texto, int maxCaracteres)
        {
            var partes = new List<string>();
            var oraciones = texto.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            var actual = new StringBuilder();

            foreach (var oracion in oraciones)
            {
                string oracionConPunto = oracion.Trim() + ".";
                if (actual.Length + oracionConPunto.Length > maxCaracteres)
                {
                    partes.Add(actual.ToString().Trim());
                    actual.Clear();
                }
                actual.Append(oracionConPunto).Append(" ");
            }

            if (actual.Length > 0)
                partes.Add(actual.ToString().Trim());

            return partes;
        }

        // ... (puedes dejar aquí el resto de tus métodos auxiliares, como los de PowerPoint, si los usas)
        public void CrearPresentacionPowerPoint(string contenido, string titulo, string carpetaDestino)
        {
            var pptApp = new Microsoft.Office.Interop.PowerPoint.Application();
            var presentacion = pptApp.Presentations.Add();

            // Paleta de colores profesionales  
            var coloresFondo = new List<int>
    {
        ColorTranslator.ToOle(Color.FromArgb(31, 78, 121)),  // Azul oscuro corporativo  
        ColorTranslator.ToOle(Color.FromArgb(58, 124, 165)), // Azul medio  
        ColorTranslator.ToOle(Color.FromArgb(0, 84, 147)),   // Azul corporativo  
        ColorTranslator.ToOle(Color.FromArgb(0, 112, 192)),  // Azul claro corporativo  
        ColorTranslator.ToOle(Color.FromArgb(22, 54, 92))    // Azul noche  
    };

            // 1. Diapositiva de título (diseño profesional)  
            var slideTitulo = presentacion.Slides.Add(1, Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutTitle);
            slideTitulo.FollowMasterBackground = Microsoft.Office.Core.MsoTriState.msoFalse;

            // Fondo con textura sutil  
            slideTitulo.Background.Fill.ForeColor.RGB = coloresFondo[0];
            slideTitulo.Background.Fill.BackColor.RGB = coloresFondo[2];
            slideTitulo.Background.Fill.TwoColorGradient(
                Microsoft.Office.Core.MsoGradientStyle.msoGradientHorizontal, 1);

            // Título principal (estilo profesional)  
            slideTitulo.Shapes[1].TextFrame.TextRange.Text = titulo;
            slideTitulo.Shapes[1].TextFrame.TextRange.Font.Size = 48;
            slideTitulo.Shapes[1].TextFrame.TextRange.Font.Bold = Microsoft.Office.Core.MsoTriState.msoTrue;
            slideTitulo.Shapes[1].TextFrame.TextRange.Font.Name = "Arial";
            slideTitulo.Shapes[1].TextFrame.TextRange.Font.Color.RGB = ColorTranslator.ToOle(Color.White);

            // Efecto de sombra para mejor legibilidad  
            slideTitulo.Shapes[1].TextFrame.TextRange.Font.Shadow = Microsoft.Office.Core.MsoTriState.msoTrue;

            // Subtítulo (opcional)  
            slideTitulo.Shapes[2].TextFrame.TextRange.Text = "Presentación Profesional";
            slideTitulo.Shapes[2].TextFrame.TextRange.Font.Size = 24;
            slideTitulo.Shapes[2].TextFrame.TextRange.Font.Color.RGB = ColorTranslator.ToOle(Color.FromArgb(200, 224, 210));

            // Diapositivas de contenido  
            string[] diapositivas = contenido.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            int maxCaracteresPorDiapositiva = 600;

            foreach (var diapositiva in diapositivas)
            {
                // Separa el título de la sección y el cuerpo
                var lineas = diapositiva.Split(new[] { '\n' }, 2, StringSplitOptions.None);
                string tituloDiapositiva = lineas[0].Replace("=", "").Trim();
                string cuerpo = lineas.Length > 1 ? lineas[1].Trim() : "";

                // Divide el cuerpo en partes si es muy largo
                var partes = DividirTextoPorPuntos(QuitarNumeraciones(cuerpo), maxCaracteresPorDiapositiva);
                for (int i = 0; i < partes.Count; i++)
                {
                    var slide = presentacion.Slides.Add(presentacion.Slides.Count + 1,
                        Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutText);

                    // Diseño alternado para cada slide  
                    int colorIndex = presentacion.Slides.Count % coloresFondo.Count;
                    slide.FollowMasterBackground = Microsoft.Office.Core.MsoTriState.msoFalse;
                    slide.Background.Fill.ForeColor.RGB = coloresFondo[colorIndex];

                    // Título con barra decorativa y "(continuación)" si es necesario
                    string tituloMostrar = partes.Count > 1
                        ? $"{tituloDiapositiva}{(i > 0 ? " (continuación)" : "")}"
                        : tituloDiapositiva;

                    slide.Shapes[1].TextFrame.TextRange.Text = tituloMostrar;
                    slide.Shapes[1].TextFrame.TextRange.Font.Size = 36;
                    slide.Shapes[1].TextFrame.TextRange.Font.Bold = Microsoft.Office.Core.MsoTriState.msoTrue;
                    slide.Shapes[1].TextFrame.TextRange.Font.Name = "Arial";
                    slide.Shapes[1].TextFrame.TextRange.Font.Color.RGB = ColorTranslator.ToOle(Color.White);

                    // Barra decorativa bajo el título  
                    var barra = slide.Shapes.AddLine(
                        slide.Shapes[1].Left, slide.Shapes[1].Top + slide.Shapes[1].Height + 5,
                        slide.Shapes[1].Left + slide.Shapes[1].Width, slide.Shapes[1].Top + slide.Shapes[1].Height + 5);
                    barra.Line.ForeColor.RGB = ColorTranslator.ToOle(Color.White);
                    barra.Line.Weight = 3;

                    // Cuerpo con estilo profesional  
                    var cuerpoShape = slide.Shapes[2].TextFrame.TextRange;
                    cuerpoShape.Text = partes[i];
                    cuerpoShape.ParagraphFormat.Bullet.Visible = Microsoft.Office.Core.MsoTriState.msoFalse;
                    cuerpoShape.Font.Size = 24;
                    cuerpoShape.Font.Name = "Calibri";
                    cuerpoShape.Font.Bold = Microsoft.Office.Core.MsoTriState.msoFalse;
                    cuerpoShape.Font.Color.RGB = ColorTranslator.ToOle(Color.White);
                    cuerpoShape.ParagraphFormat.Alignment = Microsoft.Office.Interop.PowerPoint.PpParagraphAlignment.ppAlignJustify;

                    // Resaltar subtítulos  
                    var matches = System.Text.RegularExpressions.Regex.Matches(
                        cuerpoShape.Text, @"(^|\.\s*)([^\.:]+:)", System.Text.RegularExpressions.RegexOptions.Multiline);
                    foreach (System.Text.RegularExpressions.Match match in matches)
                    {
                        int start = match.Index + match.Value.LastIndexOf(match.Groups[2].Value);
                        int length = match.Groups[2].Value.Length;
                        var rango = cuerpoShape.Characters(start + 1, length);
                        rango.Font.Bold = Microsoft.Office.Core.MsoTriState.msoTrue;
                        rango.Font.Color.RGB = ColorTranslator.ToOle(Color.FromArgb(255, 255, 180)); // Amarillo claro  
                    }
                }
            }

            // Diapositiva de cierre profesional  
            var slideCierre = presentacion.Slides.Add(
                presentacion.Slides.Count + 1,
                Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutTitle);

            slideCierre.FollowMasterBackground = Microsoft.Office.Core.MsoTriState.msoFalse;
            slideCierre.Background.Fill.ForeColor.RGB = coloresFondo[0];

            slideCierre.Shapes[1].TextFrame.TextRange.Text = "¡Gracias!";
            slideCierre.Shapes[1].TextFrame.TextRange.Font.Size = 54;
            slideCierre.Shapes[1].TextFrame.TextRange.Font.Color.RGB = ColorTranslator.ToOle(Color.White);

            slideCierre.Shapes[2].TextFrame.TextRange.Text = titulo + "\n" + DateTime.Now.ToString("dd MMMM yyyy");
            slideCierre.Shapes[2].TextFrame.TextRange.Font.Size = 20;
            slideCierre.Shapes[2].TextFrame.TextRange.Font.Color.RGB = ColorTranslator.ToOle(Color.White);

            // Guardar presentación  
            string ruta = Path.Combine(carpetaDestino, $"{LimpiarNombreArchivo(titulo)}.pptx");

            presentacion.SaveAs(ruta);
            presentacion.Close();
            pptApp.Quit();
        }


        // Divide el texto en partes de máximo 'maxCaracteres' sin cortar palabras  
        private List<string> DividirTextoEnPartes(string texto, int maxCaracteres)
        {
            var partes = new List<string>();
            var palabras = texto.Split(' ');
            var actual = new StringBuilder();

            foreach (var palabra in palabras)
            {
                if (actual.Length + palabra.Length + 1 > maxCaracteres)
                {
                    partes.Add(actual.ToString().Trim());
                    actual.Clear();
                }
                actual.Append(palabra).Append(" ");
            }

            if (actual.Length > 0)
                partes.Add(actual.ToString().Trim());

            return partes;
        }

        // Quita numeraciones tipo "1.", "2.", etc.
        public string QuitarNumeraciones(string texto)
        {
            var lineas = texto.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var resultado = new List<string>();
            foreach (var l in lineas)
            {
                string linea = l.Trim();
                if (System.Text.RegularExpressions.Regex.IsMatch(linea, @"^\d+\.\s*"))
                    linea = System.Text.RegularExpressions.Regex.Replace(linea, @"^\d+\.\s*", "");
                resultado.Add(linea);
            }
            return string.Join(Environment.NewLine, resultado);
        }

        private void ApplyBulletStyle(PP.TextFrame textFrame)
        {
            textFrame.TextRange.Font.Name = "Georgia";
            textFrame.TextRange.Font.Size = 24;
            textFrame.TextRange.ParagraphFormat.Bullet.Type = PP.PpBulletType.ppBulletUnnumbered;
            textFrame.TextRange.ParagraphFormat.Bullet.Font.Name = "Georgia";
            textFrame.TextRange.ParagraphFormat.Bullet.Character = 8226; // • bullet character
            textFrame.TextRange.ParagraphFormat.Bullet.RelativeSize = 1;
            textFrame.TextRange.ParagraphFormat.SpaceAfter = 6;
            textFrame.MarginBottom = 20;
        }
    }
}