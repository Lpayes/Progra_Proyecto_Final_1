using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1LesterFinalProgra.Config
{
    public class AppSettings
    {
        public OpenAISettings OpenAI { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
    }

    public class OpenAISettings
    {
        public string ApiKey { get; set; }
    }

    public class ConnectionStrings
    {
        public string SqlConnection { get; set; }
    }
}