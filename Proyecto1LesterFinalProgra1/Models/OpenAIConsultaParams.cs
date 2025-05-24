using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1LesterFinalProgra.Models
{
    public class OpenAIConsultaParams
    {
        public string Prompt { get; set; }
        public int MaxTokens { get; set; } = 1000;
        public double Temperature { get; set; } = 0.7;
        public double TopP { get; set; } = 0.9;
        public double FrequencyPenalty { get; set; } = 0.5;
        public double PresencePenalty { get; set; } = 0.5;
        public string Model { get; set; } = "gpt-3.5-turbo";
    }
}