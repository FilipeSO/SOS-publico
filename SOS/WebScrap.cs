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
        private static HttpClientHandler handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        static HttpClient client = new HttpClient(handler);
        #region WEBSCRAP ONS MPO
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

            var resultPost = await client.PostAsync("http://ons.org.br/_vti_bin/client.svc/ProcessQuery", httpContent);

            var resultContent = await resultPost.Content.ReadAsStringAsync();

            string corteInicio = "\"_Child_Items_\":[";
            resultContent = "{" + resultContent.Substring(resultContent.IndexOf(corteInicio));
            resultContent = resultContent.Substring(0, resultContent.LastIndexOf("]") - 1).Trim();
            ModelMPO documentos = JsonConvert.DeserializeObject<ModelMPO>(resultContent);
            return documentos;
        }
        #endregion WEBSCRAP ONS MPO

        #region ONS DIAGRAMAS
        //public static async Task ScrapOnsDiagramasAsync(string agente)
        //{
        //    //var diagramas = await GetModelDigramas();

        //}

        public static async void GetModelDigramas()
        {
            //dar um jeito de conseguir os cookies de login
            client.DefaultRequestHeaders.Add("Host", "cdre.ons.org.br");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.109 Safari/537.36");
            client.DefaultRequestHeaders.Add("Cookie", " .ONSAUTH_PROD=D4F2F09CA9F465F271FF1D656C742A920471146DD025693CCFDE82B18BEB768EBAA905E09E070FEF8287542BC552B482D25E8C0CC987927A676649EFF08F52C83C92AAD53D4ABB25BF015D0170CFAAF27208BEE534AF8D87CCC3AD436B0C4CE867FBE89F5E6EDF850FC32A005AD59B62C1AA038A; FedAuth=77u/PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz48U1A+MMe5LnR8cG9wZmVkZXJhdGlvbnByb3ZpZGVyfGZzYW9saXZlLDDHuS50fHBvcGZlZGVyYXRpb25wcm92aWRlcnxmc2FvbGl2ZSwxMzE5NTY4NDMwNzg3MDk1NDQsVHJ1ZSxENkg0RStuSHkySUNwN09vSC9vcnBwb1I2eWpqZmdZSEdpdThZWnVQZjlqeEpFSDAyUmRrNk94cmNMT2RxZHNQOVRyNythaU5LVDk1ajkzS1RjbmpqT2RhcXljMmtIelV0cmRrVDd1ZmhhRkEzaTFFQVdHUGFrb3dCcThXMnF0VWcyMmFuWWNpbFIzMElvSUdYMFpFc0VFRmxmL0tDTTVjdTRua2hnaUhrNndaSUgyMU5KS0gyaEN3OHFHOVpHRXNqQkk0VUxhaUprRVVwWWJJaVd0SkdISlFudzQ2MUp2VVFRYXN4ZXMvY2oxNmRCamxSRnlPb1VINlQ5elU3UlB6Z2R4OHpnbEU5bVBHWW9vM3NjVkhXVVYwdmNCRzF4dzZvSTljOGcrOExaRDR2OFA3cTdmZXZLRXE2YVV1dkJBc0orY2xUUWExWVowTnljd0hVZFVpQVE9PSxodHRwczovL2NkcmUub25zLm9yZy5ici88L1NQPg==;");

            var resultPost = await client.PostAsync("https://cdre.ons.org.br/_layouts/15/inplview.aspx?List={04854BFA-7127-4E36-A4ED-3EE860D929E8}&View={C35C3494-CC49-4A71-9569-34BDE063C9B5}&ViewCount=73&IsXslView=TRUE&IsCSR=TRUE&Paged=TRUE&p_ID=1444&PageFirstRow=0", null);

            var resultContent = await resultPost.Content.ReadAsStringAsync();
        }

        #endregion ONS DIAGRAMAS
    }
}
