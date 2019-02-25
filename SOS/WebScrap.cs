using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using CefSharp.WinForms.Internals;
using System.Windows.Forms;

namespace SOS
{
    public class WebScrap
    {
        private static LinkLabel statusOutputLinkLabel = null;

        public WebScrap(LinkLabel linkLabel)
        {
            statusOutputLinkLabel = linkLabel;
        }
        private static CookieContainer cookieContainer = new CookieContainer();

        private static HttpClientHandler handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            CookieContainer = cookieContainer,
            AllowAutoRedirect = true
        };
        static HttpClient client = new HttpClient(handler);

        public class CookieAwareWebClient : WebClient
        {
            private CookieContainer m_container = cookieContainer;

            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest request = base.GetWebRequest(address);
                HttpWebRequest webRequest = request as HttpWebRequest;
                if (webRequest != null)
                {
                    webRequest.CookieContainer = m_container;
                }
                return request;
            }
        }

        private string LogText = "";

        private void UILogUpdate(string report)
        {
            LogText += $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}: {report}{Environment.NewLine}";
            report = report.Length > 100 ? $"{report.Substring(0, 100)}..." : report;
            statusOutputLinkLabel.InvokeOnUiThreadIfRequired(() => statusOutputLinkLabel.Text = $"Atualização de documentos: {report}");
        }
        public void PushLogUpdate()
        {
            File.WriteAllText($"{Environment.CurrentDirectory}/Documentos/log.txt", LogText);
            LogText = "";
        }
        

        #region WEBSCRAP ONS MPO

        public void UpdateMPO()
        {
            UILogUpdate("#Início - Procedimentos da Operação (MPO)");
            UILogUpdate("ons.org.br/ conectando...");
            List<ChildItem> docsMPO = WebScrap.ScrapMPOAsync("FURNAS").GetAwaiter().GetResult();
            FileInfo jsonInfoFile = new FileInfo($"{Environment.CurrentDirectory}/Documentos/MPO/info.json");
            if (!jsonInfoFile.Directory.Exists) Directory.CreateDirectory(jsonInfoFile.Directory.FullName);

            string jsonInfo = File.Exists(jsonInfoFile.FullName) ? File.ReadAllText(jsonInfoFile.FullName) : null;
            List<ChildItem> localDocsMPO = string.IsNullOrEmpty(jsonInfo) ? new List<ChildItem>() : JsonConvert.DeserializeObject<List<ChildItem>>(jsonInfo);

            var client = new WebClient();
            foreach (var doc in docsMPO)
            {
                UILogUpdate($"{doc.MpoCodigo} atualizando");
                string docLink = $"http://ons.org.br{doc.FileRef}";
                FileInfo docFile = new FileInfo($"{Environment.CurrentDirectory}/Documentos/MPO/{doc.MpoCodigo}.pdf");

                string revisao = localDocsMPO.Where(w => w.MpoCodigo == doc.MpoCodigo).Select(s => s._Revision).FirstOrDefault();
                if (revisao == doc._Revision)
                {
                    UILogUpdate($"{doc.MpoCodigo} já se encontra na revisão vigente");
                    continue;
                }
                try
                {
                    client.DownloadFile(docLink, docFile.FullName);
                    UILogUpdate($"{doc.MpoCodigo} atualizado da revisão {revisao} para revisão {doc._Revision} em {docFile.FullName}");
                }
                catch (Exception)
                {
                    UILogUpdate($"{doc.MpoCodigo} não foi possível atualização pelo link {docLink}");
                }
            }
            List<string> mopLinks = docsMPO.Where(w => w.MpoMopsLink != null).SelectMany(s => s.MpoMopsLink).Distinct().ToList();
            string mopDir = $"{Environment.CurrentDirectory}/Documentos/MPO/MOP";
            if (!Directory.Exists(mopDir)) Directory.CreateDirectory(mopDir);
            var mopsLocal = Directory.GetFiles(mopDir, "*.pdf").ToList();
            var mopsVigentes = mopLinks.Select(s => $"{mopDir}\\{s.Split('/').Last()}").ToList();
            mopsLocal = mopsLocal.Where(w => !mopsVigentes.Contains(w)).ToList();
            mopsLocal.ForEach(s =>
            {
                File.Delete(s);
                UILogUpdate($"{s.Split('/').Last()} não está vigente e foi apagada");
            });
            foreach (var mopLink in mopLinks)
            {
                FileInfo mopFile = new FileInfo($"{Environment.CurrentDirectory}/Documentos/MPO/MOP/{mopLink.Split('/').Last()}");
                UILogUpdate($"{mopFile.Name} atualizando");
                if (mopFile.Exists)
                {
                    UILogUpdate($"{mopFile.Name} já está disponível");
                    continue;
                }
                try
                {
                    client.DownloadFile(mopLink, mopFile.FullName);
                    UILogUpdate($"{mopFile.Name} disponível em {mopFile.FullName}");
                }
                catch (Exception)
                {
                    UILogUpdate($"{mopFile.Name} não foi possível atualização pelo link {mopLink}");
                }
            }
            string json = JsonConvert.SerializeObject(docsMPO);
            File.WriteAllText(jsonInfoFile.FullName, json);
            client.Dispose();
            UILogUpdate("#Término - Procedimentos da Operação (MPO)");
        }


        static async Task<List<ChildItem>> ScrapMPOAsync(string agente)
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

        public void UpdateDiagramasONS()
        {
            UILogUpdate("#Início - Diagramas ONS");
            UILogUpdate("cdre.org.br/ conectando...");
            List<Row> diagramasONS = WebScrap.ScrapOnsDiagramasAsync("fsaolive", "123123aA").GetAwaiter().GetResult();

            FileInfo jsonInfoFile = new FileInfo($"{Environment.CurrentDirectory}/Documentos/Diagramas/info.json");
            if (!jsonInfoFile.Directory.Exists) Directory.CreateDirectory(jsonInfoFile.Directory.FullName);

            string jsonInfo = File.Exists(jsonInfoFile.FullName) ? File.ReadAllText(jsonInfoFile.FullName) : null;
            List<Row> localDiagramas = string.IsNullOrEmpty(jsonInfo) ? new List<Row>() : JsonConvert.DeserializeObject<List<Row>>(jsonInfo);

            var client = new WebScrap.CookieAwareWebClient();

            foreach (var diagrama in diagramasONS)
            {
                UILogUpdate($"{diagrama.FileLeafRef} atualizando");
                string diagramaLink = $"https://cdre.ons.org.br{diagrama.FileRef}";

                FileInfo diagramaFile = new FileInfo($"{Environment.CurrentDirectory}/Documentos/Diagramas/{diagrama.FileLeafRef.Split('_').First()}.pdf");

                string revisao = localDiagramas.Where(w => w.FileLeafRef.Split('_').First() == diagrama.FileLeafRef.Split('_').First()).Select(s => s.Modified).FirstOrDefault();
                if (revisao == diagrama.Modified)
                {
                    UILogUpdate($"{diagrama.FileLeafRef} já se encontra na revisão vigente");
                    continue;
                }
                try
                {
                    client.DownloadFile(diagramaLink, diagramaFile.FullName);
                    UILogUpdate($"{diagrama.FileLeafRef.Split('_').First()} atualizado da modificação {revisao} para modificação {diagrama.Modified} em {diagramaFile.FullName}");
                }
                catch (Exception)
                {
                    UILogUpdate($"{diagrama.FileLeafRef.Split('_').First()} não foi possível atualização pelo link {diagramaLink}");
                }
            }
            string json = JsonConvert.SerializeObject(diagramasONS);
            File.WriteAllText(jsonInfoFile.FullName, json);
            client.Dispose();
            UILogUpdate("#Término - Diagramas ONS");
        }

        static async Task<List<Row>> ScrapOnsDiagramasAsync(string username, string password)
        {
            await DiagramasAuthCDRE(username,password);

            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.109 Safari/537.36");     
            string urlRaizDiagramas = "https://cdre.ons.org.br/_layouts/15/inplview.aspx?List={04854BFA-7127-4E36-A4ED-3EE860D929E8}&View={C35C3494-CC49-4A71-9569-34BDE063C9B5}&ViewCount=129&IsXslView=TRUE&IsCSR=TRUE";
            var resultPost = await client.PostAsync($"{urlRaizDiagramas}", null);
            var resultContent = await resultPost.Content.ReadAsStringAsync();
            ModelDiagramasONS jsonDiagramas = JsonConvert.DeserializeObject<ModelDiagramasONS>(resultContent);
            List<Row> diagramas = jsonDiagramas.Row.ToList(); //resultado de 500 em 500 registros, limitação da api oferecida pelo ons

            while (jsonDiagramas.NextHref != null)
            {
                resultPost = await client.PostAsync($"{urlRaizDiagramas}&{jsonDiagramas.NextHref.TrimStart('?')}", null);
                resultContent = await resultPost.Content.ReadAsStringAsync();
                jsonDiagramas = JsonConvert.DeserializeObject<ModelDiagramasONS>(resultContent);
                diagramas.AddRange(jsonDiagramas.Row.ToList());
            }
            //Regex formacaoIncorreta = new Regex("_(r|R)ev.[a-zA-Z]+"); //remover diagramas nomenclatura incorreta
            //diagramas = diagramas.Where(w => !formacaoIncorreta.IsMatch(w.FileLeafRef)).ToList(); //remover diagramas nomenclatura incorreta

            Regex formatacaoCorreta = new Regex("_(r|R)ev.[0-9]+.pdf"); //manter diagramas com formacão correta
            diagramas = diagramas.Where(w => formatacaoCorreta.IsMatch(w.FileLeafRef)).ToList(); //manter diagramas com formacão correta
            
            return diagramas;
            //var cookies = handler.CookieContainer.GetCookies(new Uri("https://cdre.ons.org.br"));

            //resultPost = await client.PostAsync("https://cdre.ons.org.br/_layouts/15/inplview.aspx?List={04854BFA-7127-4E36-A4ED-3EE860D929E8}&View={C35C3494-CC49-4A71-9569-34BDE063C9B5}&ViewCount=129&IsXslView=TRUE&IsCSR=TRUE&Paged=TRUE&p_ID=1444&PageFirstRow=501&View=c35c3494-cc49-4a71-9569-34bde063c9b5", null);
            //resultContent = await resultPost.Content.ReadAsStringAsync();
            //diagramas = JsonConvert.DeserializeObject<ModelDiagramasONS>(resultContent);


            //resultPost = await client.PostAsync("https://cdre.ons.org.br/_layouts/15/inplview.aspx?List={04854BFA-7127-4E36-A4ED-3EE860D929E8}&View={C35C3494-CC49-4A71-9569-34BDE063C9B5}&ViewCount=129&IsXslView=TRUE&IsCSR=TRUE&Paged=TRUE&p_ID=2951&PageFirstRow=1001&View=c35c3494-cc49-4a71-9569-34bde063c9b5", null);
            //resultContent = await resultPost.Content.ReadAsStringAsync();
            //diagramas = JsonConvert.DeserializeObject<ModelDiagramasONS>(resultContent);

            //resultPost = await client.PostAsync("https://cdre.ons.org.br/_layouts/15/inplview.aspx?List={04854BFA-7127-4E36-A4ED-3EE860D929E8}&View={C35C3494-CC49-4A71-9569-34BDE063C9B5}&ViewCount=129&IsXslView=TRUE&IsCSR=TRUE&Paged=TRUE&p_ID=4465&PageFirstRow=1501&View=c35c3494-cc49-4a71-9569-34bde063c9b5", null);
            //resultContent = await resultPost.Content.ReadAsStringAsync();
            //diagramas = JsonConvert.DeserializeObject<ModelDiagramasONS>(resultContent);

            //resultPost = await client.PostAsync("https://cdre.ons.org.br/_layouts/15/inplview.aspx?List={04854BFA-7127-4E36-A4ED-3EE860D929E8}&View={C35C3494-CC49-4A71-9569-34BDE063C9B5}&ViewCount=129&IsXslView=TRUE&IsCSR=TRUE&Paged=TRUE&p_ID=5986&PageFirstRow=2001&View=c35c3494-cc49-4a71-9569-34bde063c9b5", null);
            //resultContent = await resultPost.Content.ReadAsStringAsync();
            //diagramas = JsonConvert.DeserializeObject<ModelDiagramasONS>(resultContent);

            //resultPost = await client.PostAsync("https://cdre.ons.org.br/_layouts/15/inplview.aspx?List={04854BFA-7127-4E36-A4ED-3EE860D929E8}&View={C35C3494-CC49-4A71-9569-34BDE063C9B5}&ViewCount=129&IsXslView=TRUE&IsCSR=TRUE&Paged=TRUE&p_ID=7350&PageFirstRow=2501&View=c35c3494-cc49-4a71-9569-34bde063c9b5", null);
            //resultContent = await resultPost.Content.ReadAsStringAsync();
            //diagramas = JsonConvert.DeserializeObject<ModelDiagramasONS>(resultContent);

        }

        static async Task DiagramasAuthCDRE(string username, string password)
        {
            string requestBody = $"username={username}&password={password}&submit.Signin=Entrar&CountLogin=0&CDRESolicitarCadastroUrl=http%3A%2F%2Fcdreweb.ons.org.br%2FCDRE%2FViews%2FSolicitarCadastro%2FSolicitarCadastro.aspx&POPAutenticacaoIntegradaUrl=https%3A%2F%2Facessointegrado.ons.org.br%2Facessointegrado%3FReturnUrl%3Dhttps%253a%252f%252fpops.ons.org.br%252fons.pop.federation%252fredirect%252f%253fwa%253dwsignin1.0%2526wtrealm%253d%252bhttps%25253a%25252f%25252fcdre.ons.org.br%25252f_trust%25252f%2526wctx%253dhttps%25253a%25252f%25252fcdre.ons.org.br%25252f_layouts%25252f15%25252fAuthenticate.aspx%25253fSource%25253d%2525252FMPODIAG%2525252FForms%2525252FDiags%2525252Easpx%2526wreply%253dhttps%25253a%25252f%25252fcdre.ons.org.br%25252f_trust%25252fdefault.aspx&PasswordRecoveryUrl=https%3A%2F%2Fpops.ons.org.br%2Fons.pop.federation%2Fpasswordrecovery%2F%3FReturnUrl%3Dhttps%253a%252f%252fpops.ons.org.br%252fons.pop.federation%252f%253fwa%253dwsignin1.0%2526wtrealm%253d%252bhttps%25253a%25252f%25252fcdre.ons.org.br%25252f_trust%25252f%2526wctx%253dhttps%25253a%25252f%25252fcdre.ons.org.br%25252f_layouts%25252f15%25252fAuthenticate.aspx%25253fSource%25253d%2525252FMPODIAG%2525252FForms%2525252FDiags%2525252Easpx%2526wreply%253dhttps%25253a%25252f%25252fcdre.ons.org.br%25252f_trust%25252fdefault.aspx";
            var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/x-www-form-urlencoded");

            //ON.AUTH_PROD COOKIE
            var resultPost = await client.PostAsync("https://pops.ons.org.br/ons.pop.federation/?wa=wsignin1.0&wtrealm=+https%3a%2f%2fcdre.ons.org.br%2f_trust%2f&wctx=https%3a%2f%2fcdre.ons.org.br%2f_layouts%2f15%2fAuthenticate.aspx%3fSource%3d%252F&wreply=https%3a%2f%2fcdre.ons.org.br%2f_trust%2fdefault.aspx", httpContent);

            //XML AUTH REDIRECT POST 
            resultPost = await client.PostAsync("https://pops.ons.org.br/ons.pop.federation/redirect/?wa=wsignin1.0&wtrealm=+https%3a%2f%2fcdre.ons.org.br%2f_trust%2f&wctx=https%3a%2f%2fcdre.ons.org.br%2f_layouts%2f15%2fAuthenticate.aspx%3fSource%3d%252F&wreply=https%3a%2f%2fcdre.ons.org.br%2f_trust%2fdefault.aspx", null);
            var content = await resultPost.Content.ReadAsStringAsync();
            var cookies = handler.CookieContainer.GetCookies(new Uri("https://cdre.ons.org.br"));

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(content);
            requestBody = "";
            foreach (XmlNode node in xml.SelectNodes("//input[@name][@value]"))
            {
                requestBody += $"{node.Attributes["name"].Value}={HttpUtility.UrlEncode(node.Attributes["value"].Value)}&";
            }
            requestBody = requestBody.TrimEnd('&');
            httpContent = new StringContent(requestBody, Encoding.UTF8, "application/x-www-form-urlencoded");

            //FED_AUTH COOKIE
            resultPost = await client.PostAsync("https://cdre.ons.org.br/_trust/", httpContent);
        }

        #endregion ONS DIAGRAMAS
    }
}
