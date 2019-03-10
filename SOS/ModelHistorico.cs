using System;
using System.Collections.Generic;

namespace SOS
{
    internal class ModelHistoricoAcesso
    {
        public Dictionary<string, object> Data { get; set; }
        public string PCMachineName { get; set; }
        public string PCUsername { get; set; }
        public bool SSC { get; set; }
        public string Indice { get; set; }
        public string Url { get; set; }
    }
    internal class ModelHistoricoUpdate
    {
        public Dictionary<string, object> Data { get; set; }
        public string PCMachineName { get; set; }
        public string PCUsername { get; set; }
        public bool Concluido { get; set; }
        public string Mensagem { get; set; }
    }
    internal class ModelHistoricoUpdateJson
    {
        public object Data { get; set; } //timestamp unix
        public string PCMachineName { get; set; }
        public string PCUsername { get; set; }
        public bool Concluido { get; set; }
        public string Mensagem { get; set; }
    }
    internal class ModelHistoricoAcessoJson
    {
        public object Data { get; set; } //timestamp unix
        public string PCMachineName { get; set; }
        public string PCUsername { get; set; }
        public bool SSC { get; set; }
        public string Indice { get; set; }
        public string Url { get; set; }
    }
}