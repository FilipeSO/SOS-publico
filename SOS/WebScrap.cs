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
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Xml.Linq;
using Path = System.IO.Path;
using System.Threading;
using System.Diagnostics;

namespace SOS
{
    public class WebScrap
    {
        public static readonly string DocDir = Path.Combine(Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory), "SOS Documentos");
        public static readonly string MpoDir = Path.Combine(DocDir, "MPO");
        public static readonly string DiagramasDir = Path.Combine(DocDir, "Diagramas");
        public static readonly string MopDir = Path.Combine(DocDir, "MPO", "MOP");
        public static readonly string MioDir = Path.Combine(DocDir, "MIO");
        public static readonly string MioRelacionadosDir = Path.Combine(DocDir, "MIO", "Relacionados");
        private static readonly string FirebaseDbRoot = "https://fso-sos.firebaseio.com/";
        public static bool IsCDREAuthenticated = false;
        private static LinkLabel StatusOutputLinkLabel { get; set; }

        public static void Init(LinkLabel linkLabel)
        {
            StatusOutputLinkLabel = linkLabel;
            if (!Directory.Exists(MpoDir)) Directory.CreateDirectory(MpoDir);
            if (!Directory.Exists(MopDir)) Directory.CreateDirectory(MopDir);
            if (!Directory.Exists(DiagramasDir)) Directory.CreateDirectory(DiagramasDir);
            if (!Directory.Exists(MioDir)) Directory.CreateDirectory(MioDir);
            if (!Directory.Exists(MioRelacionadosDir)) Directory.CreateDirectory(MioRelacionadosDir);
        }

        public static void Reset()
        {
            var jsonFiles = Directory.GetFiles(DocDir, "*.json", SearchOption.AllDirectories);
            foreach (var item in jsonFiles)
            {
                File.Delete(item);
            }
#if !DEBUG
            LogUpdate(true, "Efetuou reset de documentos");
#endif
        }

        public static CookieContainer cookieContainer = new CookieContainer();

        private static HttpClientHandler handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            CookieContainer = cookieContainer,
            AllowAutoRedirect = true
        };
        static HttpClient client = new HttpClient(handler);

        private class CookieAwareWebClient : WebClient
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

        public static void UpdateDocuments()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
#if !DEBUG
            LogUpdate(false, "Iniciou aplicação");
#endif
            LoadBookmarks();
            UpdateMPO();
            UpdateDiagramasONS();
            stopWatch.Stop();
#if !DEBUG
            LogUpdate(true, $"Término da atualização em {stopWatch.Elapsed.TotalSeconds}s");
#endif
        }

#region Log historico leitura
        public static async void HistoricoAcesso()
        {
            var resultPost = await client.GetAsync($"{ FirebaseDbRoot }/acesso.json?orderBy=%22Data%22&limitToFirst=1000");
            var content = await resultPost.Content.ReadAsStringAsync();
            var listaHistoricoAcesso = new List<ModelHistoricoAcessoJson>();

            foreach (Match match in Regex.Matches(content, "{\"D.*?}"))
            {
                listaHistoricoAcesso.Add(JsonConvert.DeserializeObject<ModelHistoricoAcessoJson>(match.Value));
            }
            listaHistoricoAcesso.ForEach(s => s.Data = UnixTimeStampToDateTime(Convert.ToDouble(s.Data)));
            listaHistoricoAcesso = listaHistoricoAcesso.OrderByDescending(o => o.Data).ToList();

            var t = new Thread(() =>
            {
                Historico historico = new Historico(listaHistoricoAcesso, "acesso");
                historico.ShowDialog();
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }
        public static async void HistoricoMaquina()
        {
            var resultPost = await client.GetAsync($"{ FirebaseDbRoot }/app.json?orderBy=%22Concluido%22&equalTo=true&limitToFirst=1000");
            var content = await resultPost.Content.ReadAsStringAsync();
            var listaHistoricoMaquina = new List<ModelHistoricoUpdateJson>();

            foreach (Match match in Regex.Matches(content, "{\"C.*?}"))
            {
                listaHistoricoMaquina.Add(JsonConvert.DeserializeObject<ModelHistoricoUpdateJson>(match.Value));
            }
            listaHistoricoMaquina.ForEach(s => s.Data = UnixTimeStampToDateTime(Convert.ToDouble(s.Data)));
            listaHistoricoMaquina = listaHistoricoMaquina.OrderByDescending(o => o.Data).ToList();

            var t = new Thread(() =>
            {
                Historico historico = new Historico(listaHistoricoMaquina, "maquina");
                historico.ShowDialog();
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
#endregion

#region Logging methods
        private static ModelHistoricoAcesso ultimoHistorico;
        public static async void LogAcesso(string url, string index, bool SSC)
        {
            ModelHistoricoAcesso historico = new ModelHistoricoAcesso
            {
                Data = new Dictionary<string, object>
                {
                    [".sv"] = "timestamp"
                },
                PCMachineName = Environment.MachineName,
                PCUsername = Environment.UserName,
                SSC = SSC,
                Url = url,
                Indice = index
            };
            if (historico != ultimoHistorico)
            {
                var jsonHistorico = JsonConvert.SerializeObject(historico);
                var httpContent = new StringContent(jsonHistorico, Encoding.UTF8, "application/json");
                var resultPost = await client.PostAsync($"{ FirebaseDbRoot }/acesso.json", httpContent);
                ultimoHistorico = historico;
            }
        }

        public static async void LogUpdate(bool concluido, string msg)
        {
            ModelHistoricoUpdate historico = new ModelHistoricoUpdate
            {
                Data = new Dictionary<string, object>
                {
                    [".sv"] = "timestamp"
                },
                PCMachineName = Environment.MachineName,
                PCUsername = Environment.UserName,
                Concluido = concluido,
                Mensagem = msg
            };
            var jsonHistorico = JsonConvert.SerializeObject(historico);
            var httpContent = new StringContent(jsonHistorico, Encoding.UTF8, "application/json");
            var resultPost = await client.PostAsync($"{ FirebaseDbRoot }/app.json", httpContent);
        }

        static void LogUpdate(string report, bool display = false)
        {
            string LogText = $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}: {report}";
            using (StreamWriter outfile = new StreamWriter(Path.Combine(DocDir, "log.txt"), AppendLog))
            {
                outfile.WriteLine(LogText);
            }
            AppendLog = true;
            report = report.Length > 100 ? $"{report.Substring(0, 100)}..." : report;
            if (display) StatusOutputLinkLabel.InvokeOnUiThreadIfRequired(() => StatusOutputLinkLabel.Text = $"Atualização de documentos: {report}");
        }
        private static bool AppendLog = false;

#endregion

#region Indexing methods
        private static HashSet<ModelSearchBookmark> LocalBookmarks;
        private static FileInfo jsonBookmarkFile = new FileInfo(Path.Combine(DocDir, "bookmarks.json"));
        private static void LoadBookmarks()
        {
            //PM.SE.3SP NUMERO 1239 para testes
            string bookmarkInfo = File.Exists(jsonBookmarkFile.FullName) ? File.ReadAllText(jsonBookmarkFile.FullName) : null;
            try
            {
                LocalBookmarks = JsonConvert.DeserializeObject<HashSet<ModelSearchBookmark>>(bookmarkInfo);
            }
            catch (Exception)
            {
            }
            if (LocalBookmarks == null) LocalBookmarks = new HashSet<ModelSearchBookmark>();
        }
        private static IEnumerable<Bookmark> GetPdfBookmark(FileInfo docFile, bool diagrama = false, string tituloDiagrama = null)
        {
            var listBookmarks = new List<Bookmark>();
            if (diagrama)
            {
                listBookmarks.Add(new Bookmark //default primeiro bookmark é o nome do próprio documento
                {
                    Title = tituloDiagrama.ToLower().IndexOf("diagrama") > -1 ? tituloDiagrama : $"Diagrama {tituloDiagrama}",
                    PathAndPage = $"{docFile.FullName}"
                });
                return listBookmarks;
            }
            else
            {
                listBookmarks.Add(new Bookmark //default primeiro bookmark é o nome do próprio documento
                {
                    Title = docFile.Name,
                    PathAndPage = $"{docFile.FullName}"
                });
            }

            PdfReader reader = new PdfReader(docFile.FullName);
            IList<Dictionary<string, object>> bookmarks = SimpleBookmark.GetBookmark(reader);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                SimpleBookmark.ExportToXML(bookmarks, memoryStream, "ISO8859-1", true);
                XDocument xml = XDocument.Parse(Encoding.ASCII.GetString(memoryStream.ToArray()));
                foreach (var node in xml.Descendants("Title"))
                {
                    string Title = node.HasElements == true ? node.FirstNode.ToString() : (string)node;
                    string Page = (string)node.Attribute("Page");
                    listBookmarks.Add(new Bookmark
                    {
                        Title = Title.TrimEnd('\r', '\n', ' '),
                        PathAndPage = $"{docFile.FullName}#page={Page.Split(' ')[0]}"
                    });
                }
                //File.WriteAllBytes(docFile.FullName.Replace(".pdf", ".xml"), memoryStream.ToArray());
            }
            return listBookmarks;
        }
#endregion

#region ONS MPO

        static void UpdateMPO()
        {
            LogUpdate("#Início - Procedimentos da Operação (MPO)", true);
            LogUpdate("ons.org.br/ conectando...", true);
            IEnumerable<ChildItem> docsMPO = ScrapMPOAsync().GetAwaiter().GetResult();
            int totalItems = docsMPO.Count();
            int counter = 1;

            FileInfo jsonInfoFile = new FileInfo(Path.Combine(MpoDir, "info.json"));

            string jsonInfo = File.Exists(jsonInfoFile.FullName) ? File.ReadAllText(jsonInfoFile.FullName) : null;

            var localDocsMPO = new List<ChildItem>();
            try //problema ao corromper gravação do json //parse arquivo json inválido
            {
                localDocsMPO = JsonConvert.DeserializeObject<List<ChildItem>>(jsonInfo);
            }
            catch (Exception)
            {
            }
            if (LocalBookmarks.Count == 0) localDocsMPO = new List<ChildItem>(); //forçar update completo caso bookmark corrompido

            bool bookmarkUpdate = false;
            var client = new WebClient();
            foreach (var doc in docsMPO)
            {
                LogUpdate($"{doc.MpoCodigo} atualizando {counter++} de {totalItems}", true);
                string docLink = $"http://ons.org.br{doc.FileRef}";
                FileInfo docFile = new FileInfo(Path.Combine(MpoDir, $"{doc.MpoCodigo}.pdf"));

                string revisao = localDocsMPO.Where(w => w.MpoCodigo == doc.MpoCodigo).Select(s => s._Revision).FirstOrDefault();
                if (revisao == doc._Revision && docFile.Exists)
                {
                    LogUpdate($"{doc.MpoCodigo} já se encontra na revisão vigente");
                    continue;
                }
                try
                {
                    client.DownloadFile(docLink, docFile.FullName);
                    //Collection of bookmarks
                    bookmarkUpdate = true;
                    var listBookmarks = GetPdfBookmark(docFile);
                    if (LocalBookmarks.Any(w => w.MpoCodigo == doc.MpoCodigo))
                    {
                        foreach (var item in LocalBookmarks)
                        {
                            if (item.MpoCodigo == doc.MpoCodigo) item.Bookmarks = listBookmarks;
                        }
                    }
                    else
                    {
                        LocalBookmarks.Add(new ModelSearchBookmark
                        {
                            MpoCodigo = doc.MpoCodigo,
                            Bookmarks = listBookmarks
                        });
                    }
                    LogUpdate($"{doc.MpoCodigo} atualizado da revisão {revisao} para revisão {doc._Revision} em {docFile.FullName}");
                }
                catch (Exception)
                {
                    LogUpdate($"{doc.MpoCodigo} não foi possível atualização pelo link {docLink}");
                }
            }
            if (bookmarkUpdate)
            {
                File.WriteAllText(jsonBookmarkFile.FullName, JsonConvert.SerializeObject(LocalBookmarks));
            }

            var mopLinks = docsMPO.Where(w => w.MpoMopsLink != null).SelectMany(s => s.MpoMopsLink).Distinct();
            var mopsVigentes = mopLinks.Select(s => Path.Combine(MopDir, Path.GetFileName(s)));
            var mopsLocal = Directory.GetFiles(MopDir, "*.pdf").Except(mopsVigentes);
            foreach (var item in mopsLocal)
            {
                File.Delete(item);
                LogUpdate($"{Path.GetFileNameWithoutExtension(item)} não está vigente e foi apagada");
            }
            totalItems = mopLinks.Count();
            counter = 1;
            foreach (var mopLink in mopLinks)
            {
                FileInfo mopFile = new FileInfo(Path.Combine(MopDir, Path.GetFileName(mopLink)));
                LogUpdate($"{mopFile.Name} atualizando {counter++} de {totalItems}", true);
                if (mopFile.Exists)
                {
                    LogUpdate($"{mopFile.Name} já está disponível");
                    continue;
                }
                try
                {
                    client.DownloadFile(mopLink, mopFile.FullName);
                    LogUpdate($"{mopFile.Name} disponível em {mopFile.FullName}");
                }
                catch (Exception)
                {
                    LogUpdate($"{mopFile.Name} não foi possível atualização pelo link {mopLink}");
                }
            }
            File.WriteAllText(jsonInfoFile.FullName, JsonConvert.SerializeObject(docsMPO));
            client.Dispose();
            LogUpdate("#Término - Procedimentos da Operação (MPO)", true);
        }

        static async Task<IEnumerable<ChildItem>> ScrapMPOAsync(string agente = null)
        {
            string requestToken = await GetRequestDigestToken();
            ModelMPO docs = await GetModelMPO(requestToken);
            var validDocs = docs._Child_Items_.Where(w => w.MpoAgentesAssinantes != null);
            var filteredDocs = string.IsNullOrWhiteSpace(agente) ? validDocs : validDocs.Where(w => w.MpoAgentesAssinantes.Contains(agente));
            filteredDocs = CompileMopLinks(filteredDocs);
            return filteredDocs;
        }

        static IEnumerable<ChildItem> CompileMopLinks(IEnumerable<ChildItem> docs)
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
            //client.DefaultRequestHeaders.Add("Host", "ons.org.br");
            client.DefaultRequestHeaders.Add("X-RequestDigest", requestDigestToken);
            //client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");

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

        static void UpdateDiagramasONS()
        {
            LogUpdate("#Início - Diagramas ONS", true);
            LogUpdate("cdre.org.br/ conectando...", true);
            IEnumerable<Row> diagramasONS = ScrapOnsDiagramasAsync().GetAwaiter().GetResult();
            int totalItems = diagramasONS.Count();
            int counter = 1;
            FileInfo jsonInfoFile = new FileInfo(Path.Combine(DiagramasDir, "info.json"));

            string jsonInfo = File.Exists(jsonInfoFile.FullName) ? File.ReadAllText(jsonInfoFile.FullName) : null;

            IEnumerable<Row> localDiagramas = new List<Row>();
            try //problema ao corromper gravação do json //parse arquivo json inválido
            {
                localDiagramas = JsonConvert.DeserializeObject<IEnumerable<Row>>(jsonInfo);
            }
            catch (Exception)
            {
            }
            if (LocalBookmarks.Count == 0) localDiagramas = new List<Row>(); //forçar update completo caso bookmark corrompido

            bool bookmarkUpdate = false;
            var client = new CookieAwareWebClient();
            foreach (var diagrama in diagramasONS)
            {
                LogUpdate($"{diagrama.FileLeafRef} atualizando {counter++} de {totalItems}", true);
                string diagramaLink = $"https://cdre.ons.org.br{diagrama.FileRef}";

                FileInfo diagramaFile = new FileInfo(Path.Combine(DiagramasDir, diagrama.FileLeafRef));

                string revisao = localDiagramas.Where(w => w.FileLeafRef == diagrama.FileLeafRef).Select(s => s.Modified).FirstOrDefault();
                if (revisao == diagrama.Modified && diagramaFile.Exists)
                {
                    LogUpdate($"{diagrama.FileLeafRef} já se encontra na revisão vigente");
                    continue;
                }
                try
                {
                    client.DownloadFile(diagramaLink, diagramaFile.FullName);
                    bookmarkUpdate = true;
                    var listBookmarks = GetPdfBookmark(diagramaFile, true, diagrama.MpoAssunto);
                    if (LocalBookmarks.Any(w => w.MpoCodigo == diagrama.FileLeafRef))
                    {
                        foreach (var item in LocalBookmarks)
                        {
                            if (item.MpoCodigo == diagrama.FileLeafRef) item.Bookmarks = listBookmarks;
                        }
                    }
                    else
                    {
                        LocalBookmarks.Add(new ModelSearchBookmark
                        {
                            MpoCodigo = diagrama.FileLeafRef,
                            Bookmarks = listBookmarks
                        });
                    }
                    LogUpdate($"{diagrama.FileLeafRef} atualizado da modificação {revisao} para modificação {diagrama.Modified} em {diagramaFile.FullName}");
                }
                catch (Exception)
                {
                    LogUpdate($"{diagrama.FileLeafRef} não foi possível atualização pelo link {diagramaLink}");
                }
            }
            if (bookmarkUpdate)
            {
                File.WriteAllText(jsonBookmarkFile.FullName, JsonConvert.SerializeObject(LocalBookmarks));
            }
            var diagramasVigentes = diagramasONS.Select(s => $"{jsonInfoFile.Directory.FullName}\\{s.FileLeafRef}");
            var diagramasLocal = Directory.GetFiles(jsonInfoFile.Directory.FullName, "*.pdf").Except(diagramasVigentes);
            foreach (var item in diagramasLocal)
            {
                File.Delete(item);
                LogUpdate($"{Path.GetFileNameWithoutExtension(item)} não está vigente e foi apagado");
            }
            File.WriteAllText(jsonInfoFile.FullName, JsonConvert.SerializeObject(diagramasONS));
            client.Dispose();
            LogUpdate("#Término - Diagramas ONS", true);
        }

        static async Task<IEnumerable<Row>> ScrapOnsDiagramasAsync()
        {
            //await DiagramasAuthCDRE(username, password); etapa realizada pelo login

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
                diagramas.AddRange(jsonDiagramas.Row);
            }

            return diagramas;
        }

        public static async Task DiagramasAuthCDRE(string username, string password)
        {

            string requestBody = $"username={username}&password={password}&submit.Signin=Entrar&CountLogin=0&CDRESolicitarCadastroUrl=http%3A%2F%2Fcdreweb.ons.org.br%2FCDRE%2FViews%2FSolicitarCadastro%2FSolicitarCadastro.aspx&POPAutenticacaoIntegradaUrl=https%3A%2F%2Facessointegrado.ons.org.br%2Facessointegrado%3FReturnUrl%3Dhttps%253a%252f%252fpops.ons.org.br%252fons.pop.federation%252fredirect%252f%253fwa%253dwsignin1.0%2526wtrealm%253d%252bhttps%25253a%25252f%25252fcdre.ons.org.br%25252f_trust%25252f%2526wctx%253dhttps%25253a%25252f%25252fcdre.ons.org.br%25252f_layouts%25252f15%25252fAuthenticate.aspx%25253fSource%25253d%2525252FMPODIAG%2525252FForms%2525252FDiags%2525252Easpx%2526wreply%253dhttps%25253a%25252f%25252fcdre.ons.org.br%25252f_trust%25252fdefault.aspx&PasswordRecoveryUrl=https%3A%2F%2Fpops.ons.org.br%2Fons.pop.federation%2Fpasswordrecovery%2F%3FReturnUrl%3Dhttps%253a%252f%252fpops.ons.org.br%252fons.pop.federation%252f%253fwa%253dwsignin1.0%2526wtrealm%253d%252bhttps%25253a%25252f%25252fcdre.ons.org.br%25252f_trust%25252f%2526wctx%253dhttps%25253a%25252f%25252fcdre.ons.org.br%25252f_layouts%25252f15%25252fAuthenticate.aspx%25253fSource%25253d%2525252FMPODIAG%2525252FForms%2525252FDiags%2525252Easpx%2526wreply%253dhttps%25253a%25252f%25252fcdre.ons.org.br%25252f_trust%25252fdefault.aspx";
            var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/x-www-form-urlencoded");

            //ON.AUTH_PROD COOKIE
            var resultPost = await client.PostAsync("https://pops.ons.org.br/ons.pop.federation/?wa=wsignin1.0&wtrealm=+https%3a%2f%2fcdre.ons.org.br%2f_trust%2f&wctx=https%3a%2f%2fcdre.ons.org.br%2f_layouts%2f15%2fAuthenticate.aspx%3fSource%3d%252F&wreply=https%3a%2f%2fcdre.ons.org.br%2f_trust%2fdefault.aspx", httpContent);

            //XML AUTH REDIRECT POST 
            resultPost = await client.PostAsync("https://pops.ons.org.br/ons.pop.federation/redirect/?wa=wsignin1.0&wtrealm=+https%3a%2f%2fcdre.ons.org.br%2f_trust%2f&wctx=https%3a%2f%2fcdre.ons.org.br%2f_layouts%2f15%2fAuthenticate.aspx%3fSource%3d%252F&wreply=https%3a%2f%2fcdre.ons.org.br%2f_trust%2fdefault.aspx", null);
            var content = await resultPost.Content.ReadAsStringAsync();
            requestBody = "";
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(content);
                foreach (XmlNode node in xml.SelectNodes("//input[@name][@value]"))
                {
                    requestBody += $"{node.Attributes["name"].Value}={HttpUtility.UrlEncode(node.Attributes["value"].Value)}&";
                }
                requestBody = requestBody.TrimEnd('&');
            }
            catch (Exception)
            {
                //xml parse failure
            }
            httpContent = new StringContent(requestBody, Encoding.UTF8, "application/x-www-form-urlencoded");

            //FED_AUTH COOKIE
            resultPost = await client.PostAsync("https://cdre.ons.org.br/_trust/", httpContent);
            if (cookieContainer.Count == 3) IsCDREAuthenticated = true;
        }

#endregion ONS DIAGRAMAS
    }
}
