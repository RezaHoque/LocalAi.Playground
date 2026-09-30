using System;
using System.Collections.Generic;
using System.Text;

namespace LocalAI.Playground.Models
{
    public class OllamaResponse
    {
        public OllamaMessage? Message { get; set; }
        public bool Done { get; set; }
    }
}
