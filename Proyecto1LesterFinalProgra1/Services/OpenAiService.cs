using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Proyecto1LesterFinalProgra.Models;

namespace Proyecto1LesterFinalProgra.Services
{
    public class OpenAiService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public OpenAiService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<(string respuesta, int promptTokens, int totalTokens)> HacerConsultaAsync(OpenAIConsultaParams parametros)
        {
            var requestBody = new
            {
                model = parametros.Model,
                messages = new[]
                {
                    new { role = "system", content = "Eres un asistente académico que genera respuestas detalladas, bien estructuradas y profesionales." },
                    new { role = "user", content = parametros.Prompt }
                },
                max_tokens = parametros.MaxTokens,
                temperature = parametros.Temperature,
                top_p = parametros.TopP,
                frequency_penalty = parametros.FrequencyPenalty,
                presence_penalty = parametros.PresencePenalty
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var openAiResponse = JsonSerializer.Deserialize<OpenAIResponse>(responseString);

            string respuesta = openAiResponse?.choices?[0]?.message?.content?.Trim() ?? "No se recibió respuesta válida.";
            int promptTokens = openAiResponse?.usage?.prompt_tokens ?? 0;
            int totalTokens = openAiResponse?.usage?.total_tokens ?? 0;

            return (respuesta, promptTokens, totalTokens);
        }

        public async Task<string> GenerarTituloAsync(string texto)
        {
            string promptTitulo = $"Genera un título académico profesional en español para un documento sobre: {texto}. El título debe ser claro, conciso y máximo 10 palabras.";

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "Genera títulos concisos y profesionales para documentos académicos." },
                    new { role = "user", content = promptTitulo }
                },
                max_tokens = 50,
                temperature = 0.3
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var openAiResponse = JsonSerializer.Deserialize<OpenAIResponse>(responseString);

            return openAiResponse?.choices?[0]?.message?.content?.Trim('"', '\'') ?? "Título generado";
        }

        public async Task<string> GenerarResumenAsync(string prompt)
        {
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "Genera resúmenes ejecutivos concisos de aproximadamente 100 palabras." },
                    new { role = "user", content = prompt }
                },
                max_tokens = 200,
                temperature = 0.5
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var openAiResponse = JsonSerializer.Deserialize<OpenAIResponse>(responseString);

            return openAiResponse?.choices?[0]?.message?.content ?? "Resumen generado";
        }
    }
}