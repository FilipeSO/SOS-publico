using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;
using SOS.Handlers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOS
{
    public partial class BrowserInterface : Form
    {
        // Default to a small increment:
        private const double ZoomIncrement = 0.10;
        private const string DefaultUrlForAddedTabs = "https://www.google.com";
        private bool multiThreadedMessageLoopEnabled;

        public BrowserInterface(bool multiThreadedMessageLoopEnabled)
        {
            InitializeComponent();

            var bitness = Environment.Is64BitProcess ? "x64" : "x86";
            Text = "SOS - " + bitness;
            WindowState = FormWindowState.Maximized;

            Load += BrowserInterface_Load;
            //Only perform layout when control has completly finished resizing
            ResizeBegin += (s, e) => SuspendLayout();
            ResizeEnd += (s, e) => ResumeLayout(true);

            this.multiThreadedMessageLoopEnabled = multiThreadedMessageLoopEnabled;

        }

        private void BrowserInterface_Load(object sender, EventArgs e)
        {

            FileInfo pdfFile = new FileInfo($"{Environment.CurrentDirectory}/Documentos/MPO/AO-AJ.SE.UHAT.pdf");
            AddTab(pdfFile.FullName);

            Task.Run(() =>
            {
                //UpdateMPO();
                //FileLogUpdate();
                //UpdateDiagramasONS();
                //FileLogUpdate();

            });
        }

        private void AddTab(string url, int? insertIndex = null)
        {
            browserTabControl.SuspendLayout();

            var browser = new BrowserTabUserControl(AddTab, url, multiThreadedMessageLoopEnabled)
            {
                Dock = DockStyle.Fill,
            };

            var tabPage = new TabPage(url)
            {
                Dock = DockStyle.Fill
            };

            //This call isn't required for the sample to work. 
            //It's sole purpose is to demonstrate that #553 has been resolved.
            browser.CreateControl();

            tabPage.Controls.Add(browser);

            if (insertIndex == null)
            {
                browserTabControl.TabPages.Add(tabPage);
            }
            else
            {
                browserTabControl.TabPages.Insert(insertIndex.Value, tabPage);
            }

            //Make newly created tab active
            browserTabControl.SelectedTab = tabPage;

            browserTabControl.ResumeLayout(true);
        }

        private void ExitMenuItemClick(object sender, EventArgs e)
        {
            ExitApplication();
        }

        private void ExitApplication()
        {
            Close();
        }

        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }

        public void RemoveTab(IntPtr windowHandle)
        {
            var parentControl = FromChildHandle(windowHandle);
            if (!parentControl.IsDisposed)
            {
                if (parentControl.Parent is TabPage tabPage)
                {
                    browserTabControl.TabPages.Remove(tabPage);
                }
                else if (parentControl.Parent is Panel panel)
                {
                    var browserTabUserControl = (BrowserTabUserControl)panel.Parent;

                    var tab = (TabPage)browserTabUserControl.Parent;
                    browserTabControl.TabPages.Remove(tab);
                }
            }
        }

        private void FindMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.ShowFind();
            }
        }

        private void CopySourceToClipBoardAsyncClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.CopySourceToClipBoardAsync();
            }
        }

        private BrowserTabUserControl GetCurrentTabControl()
        {
            if (browserTabControl.SelectedIndex == -1)
            {
                return null;
            }

            var tabPage = browserTabControl.Controls[browserTabControl.SelectedIndex];
            var control = tabPage.Controls[0] as BrowserTabUserControl;

            return control;
        }

        private void NewTabToolStripMenuItemClick(object sender, EventArgs e)
        {
            AddTab(DefaultUrlForAddedTabs);
        }

        private void CloseTabToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (browserTabControl.TabPages.Count == 0)
            {
                return;
            }

            var currentIndex = browserTabControl.SelectedIndex;

            var tabPage = browserTabControl.TabPages[currentIndex];

            var control = GetCurrentTabControl();
            if (control != null && !control.IsDisposed)
            {
                control.Dispose();
            }

            browserTabControl.TabPages.Remove(tabPage);

            tabPage.Dispose();

            browserTabControl.SelectedIndex = currentIndex - 1;

            if (browserTabControl.TabPages.Count == 0)
            {
                ExitApplication();
            }
        }

        private void UndoMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Undo();
            }
        }

        private void RedoMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Redo();
            }
        }

        private void CutMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Cut();
            }
        }

        private void CopyMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Copy();
            }
        }

        private void PasteMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Paste();
            }
        }

        private void DeleteMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Delete();
            }
        }

        private void SelectAllMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.SelectAll();
            }
        }

        private void PrintToolStripMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Print();
            }
        }

        private void ShowDevToolsMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.ShowDevTools();

                //EXPERIMENTAL Example below shows how to use a control to host DevTools
                //(in this case it's added as a new TabPage)
                // NOTE: Does not currently move/resize correctly
                //var tabPage = new TabPage("DevTools")
                //{
                //    Dock = DockStyle.Fill
                //};

                //var panel = new Panel
                //{
                //    Dock = DockStyle.Fill
                //};

                ////We need to call CreateControl as we need the Handle later
                //panel.CreateControl();

                //tabPage.Controls.Add(panel);

                //browserTabControl.TabPages.Add(tabPage);

                ////Make newly created tab active
                //browserTabControl.SelectedTab = tabPage;

                ////Grab the client rect
                //var rect = panel.ClientRectangle;
                //var webBrowser = control.Browser;
                //var browser = webBrowser.GetBrowser().GetHost();
                //var windowInfo = new WindowInfo();
                ////DevTools becomes a child of the panel, we use it's dimesions
                //windowInfo.SetAsChild(panel.Handle, rect.Left, rect.Top, rect.Right, rect.Bottom);
                ////Show DevTools in our panel 
                //browser.ShowDevTools(windowInfo);
            }
        }

        private void CloseDevToolsMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.CloseDevTools();
            }
        }

        private void ZoomInToolStripMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                var task = control.Browser.GetZoomLevelAsync();

                task.ContinueWith(previous =>
                {
                    if (previous.Status == TaskStatus.RanToCompletion)
                    {
                        var currentLevel = previous.Result;
                        control.Browser.SetZoomLevel(currentLevel + ZoomIncrement);
                    }
                    else
                    {
                        throw new InvalidOperationException("Unexpected failure of calling CEF->GetZoomLevelAsync", previous.Exception);
                    }
                }, TaskContinuationOptions.ExecuteSynchronously);
            }
        }

        private void ZoomOutToolStripMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                var task = control.Browser.GetZoomLevelAsync();
                task.ContinueWith(previous =>
                {
                    if (previous.Status == TaskStatus.RanToCompletion)
                    {
                        var currentLevel = previous.Result;
                        control.Browser.SetZoomLevel(currentLevel - ZoomIncrement);
                    }
                    else
                    {
                        throw new InvalidOperationException("Unexpected failure of calling CEF->GetZoomLevelAsync", previous.Exception);
                    }
                }, TaskContinuationOptions.ExecuteSynchronously);
            }
        }

        private void CurrentZoomLevelToolStripMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                var task = control.Browser.GetZoomLevelAsync();
                task.ContinueWith(previous =>
                {
                    if (previous.Status == TaskStatus.RanToCompletion)
                    {
                        var currentLevel = previous.Result;
                        MessageBox.Show("Current ZoomLevel: " + currentLevel.ToString());
                    }
                    else
                    {
                        MessageBox.Show("Unexpected failure of calling CEF->GetZoomLevelAsync: " + previous.Exception.ToString());
                    }
                }, TaskContinuationOptions.HideScheduler);
            }
        }

        private void GoToDemoPageToolStripMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Load("custom://cefsharp/ScriptedMethodsTest.html");
            }
        }

        private async void PrintToPdfToolStripMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                var dialog = new SaveFileDialog
                {
                    DefaultExt = ".pdf",
                    Filter = "Pdf documents (.pdf)|*.pdf"
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var success = await control.Browser.PrintToPdfAsync(dialog.FileName, new PdfPrintSettings
                    {
                        MarginType = CefPdfPrintMarginType.Custom,
                        MarginBottom = 10,
                        MarginTop = 0,
                        MarginLeft = 20,
                        MarginRight = 10
                    });

                    if (success)
                    {
                        MessageBox.Show("Pdf was saved to " + dialog.FileName);
                    }
                    else
                    {
                        MessageBox.Show("Unable to save Pdf, check you have write permissions to " + dialog.FileName);
                    }

                }

            }
        }

        private void OpenDataUrlToolStripMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                const string html = "<html><head><title>Test</title></head><body><h1>Html Encoded in URL!</h1></body></html>";
                control.Browser.LoadHtml(html, false);
            }
        }

        private void OpenHttpBinOrgToolStripMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Load("https://httpbin.org/");
            }
        }

        private void RunFileDialogToolStripMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.GetBrowserHost().RunFileDialog(CefFileDialogMode.Open, "Open", null, new List<string> { "*.*" }, 0, new RunFileDialogCallback());
            }
        }

        private void LoadExtensionsToolStripMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                //The sample extension only works for http(s) schemes
                if (control.Browser.Address.StartsWith("http"))
                {
                    var requestContext = control.Browser.GetBrowserHost().RequestContext;

                    var dir = Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\CefSharp.Example\Extensions");
                    dir = Path.GetFullPath(dir);
                    if (!Directory.Exists(dir))
                    {
                        throw new DirectoryNotFoundException("Unable to locate example extensions folder - " + dir);
                    }

                    var extensionHandler = new ExtensionHandler
                    {
                        LoadExtensionPopup = (url) =>
                        {
                            BeginInvoke(new Action(() =>
                            {
                                var extensionForm = new Form();

                                var extensionBrowser = new ChromiumWebBrowser(url);
                                //extensionBrowser.IsBrowserInitializedChanged += (s, args) =>
                                //{
                                //    extensionBrowser.ShowDevTools();
                                //};

                                extensionForm.Controls.Add(extensionBrowser);

                                extensionForm.Show(this);
                            }));
                        },
                        GetActiveBrowser = (extension, isIncognito) =>
                        {
                            //Return the active browser for which the extension will act upon
                            return control.Browser.GetBrowser();
                        }
                    };

                    requestContext.LoadExtensionsFromDirectory(dir, extensionHandler);
                }
                else
                {
                    MessageBox.Show("The sample extension only works with http(s) schemes, please load a different website and try again", "Unable to load Extension");
                }
            }
        }
        #region UPDATE

        private string LogText = "";

        private void UILogUpdate(string report)
        {
            LogText += $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}: {report}{Environment.NewLine}";
            report = report.Length > 100 ? $"{report.Substring(0, 100)}..." : report;
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    //toolStripStatusLabel1.Text = $"Atualização de documentos: {report}";
                }));
            }
        }
        private void FileLogUpdate()
        {
            File.WriteAllText($"{Environment.CurrentDirectory}/Documentos/log.txt", LogText);
        }

        private void UpdateDiagramasONS()
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
                
                string revisao = localDiagramas.Where(w=>w.FileLeafRef.Split('_').First() == diagrama.FileLeafRef.Split('_').First()).Select(s=>s.Modified).FirstOrDefault();
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

        private void UpdateMPO()
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
            mopsLocal = mopsLocal.Where(w=>!mopsVigentes.Contains(w)).ToList();
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
        #endregion UPDATE
    }
}
