using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SOS
{
    static class WebScrap
    {

        static readonly HttpClientHandler handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        static HttpClient client = new HttpClient(handler);

        public static async Task<List<ChildItem>> ScrapMPOAsync(string agente)
        {
            string requestToken = await GetRequestDigestToken();
            ModelMPO docs = await GetModelMPO(requestToken);
            var validDocs = docs._Child_Items_.Where(w => w.MpoAgentesAssinantes != null);
            var filteredDocs = validDocs.Where(w => w.MpoAgentesAssinantes.Contains(agente)).ToList();
            filteredDocs = CompileMopLinks(filteredDocs);
            return filteredDocs;
        }

        static List<ChildItem> CompileMopLinks(List<ChildItem> docs)
        {
            foreach (var item in docs.Where(w => w.MpoAlteradosPelasMops != null))
            {
                item.MpoMopsLink = new List<string>();
                foreach (var mop in item.MpoAlteradosPelasMops.ToString().Split(',')) //formato: MOP/ONS xxx-R/2018,MOP/ONS xxx-S/2019 
                {
                    string tag = mop.Contains("R") == true ? "Regional" : "Sistêmica";
                    string responsavel = item.MpoResponsavel;
                    string fileMOP = mop.Replace('/', '-');
                    item.MpoMopsLink.Add($"http://www.ons.org.br/MPO/Mensagem Operativa/{tag}/{responsavel}/{fileMOP}.pdf");
                }                
            }
            return docs;
        }

        static async Task<string> GetRequestDigestToken()
        {
            var getResult = await client.GetAsync("http://ons.org.br/paginas/sobre-o-ons/procedimentos-de-rede/mpo");
            var html = await getResult.Content.ReadAsStringAsync();
            var requestDigestToken = Regex.Match(html, @"id=""__REQUESTDIGEST"" value=""(.*)""").Groups[1].Value;
            return requestDigestToken;
        }


        static async Task<ModelMPO> GetModelMPO(string requestDigestToken)
        {
            client.DefaultRequestHeaders.Add("Host", "ons.org.br");
            client.DefaultRequestHeaders.Add("X-RequestDigest", requestDigestToken);
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");

            string requestBody = @"<Request xmlns=""http://schemas.microsoft.com/sharepoint/clientquery/2009"" SchemaVersion=""15.0.0.0"" LibraryVersion=""16.0.0.0"" ApplicationName=""Javascript Library""><Actions><Query Id=""24"" ObjectPathId=""11""><Query SelectAllProperties=""true""><Properties /></Query><ChildItemQuery SelectAllProperties=""true""><Properties /></ChildItemQuery></Query></Actions><ObjectPaths><Method Id=""11"" ParentId=""8"" Name=""GetItems""><Parameters><Parameter TypeId=""{3d248d7b-fc86-40a3-aa97-02a75d69fb8a}""><Property Name=""DatesInUtc"" Type=""Boolean"">true</Property><Property Name=""FolderServerRelativeUrl"" Type=""String"">/MPO/Documento Normativo</Property><Property Name=""ListItemCollectionPosition"" Type=""Null"" /><Property Name=""ViewXml"" Type=""String"">&lt;View Scope='RecursiveAll'&gt;   &lt;Query&gt;       &lt;OrderBy&gt;       	&lt;FieldRef Name='Title' /&gt;       	&lt;FieldRef Name='MpoAssunto' /&gt;       	&lt;FieldRef Name='FileLeafRef' /&gt;   	&lt;/OrderBy&gt;   &lt;/Query&gt;&lt;/View&gt;</Property></Parameter></Parameters></Method><Method Id=""8"" ParentId=""6"" Name=""GetByTitle""><Parameters><Parameter Type=""String"">MPO</Parameter></Parameters></Method><Property Id=""6"" ParentId=""4"" Name=""Lists"" /><Property Id=""4"" ParentId=""2"" Name=""RootWeb"" /><Property Id=""2"" ParentId=""0"" Name=""Site"" /><StaticProperty Id=""0"" TypeId=""{3747adcd-a3c3-41b9-bfab-4a64dd2f1e0a}"" Name=""Current"" /></ObjectPaths></Request>";
            var httpContent = new StringContent(requestBody, Encoding.UTF8, "text/xml");

            //var resultPost = await client.PostAsync("/_vti_bin/client.svc/ProcessQuery", httpContent);
            var resultPost = await client.PostAsync("http://ons.org.br/_vti_bin/client.svc/ProcessQuery", httpContent);

            var resultContent = await resultPost.Content.ReadAsStringAsync();

            string corteInicio = "\"_Child_Items_\":[";
            resultContent = "{" + resultContent.Substring(resultContent.IndexOf(corteInicio));
            resultContent = resultContent.Substring(0, resultContent.LastIndexOf("]") - 1).Trim();
            ModelMPO documentos = JsonConvert.DeserializeObject<ModelMPO>(resultContent);
            return documentos;
        }

    }
}
