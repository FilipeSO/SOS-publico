using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Internals;
using Newtonsoft.Json;
using SOS.Handlers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        private string[] DocumentDirectories = new string[] { Path.Combine(Environment.CurrentDirectory, "Documentos", "MPO"), Path.Combine(Environment.CurrentDirectory, "Documentos", "MPO", "MOP"), Path.Combine(Environment.CurrentDirectory, "Documentos", "Diagramas") };

        private bool multiThreadedMessageLoopEnabled;

        public BrowserInterface(bool multiThreadedMessageLoopEnabled)
        {
            InitializeComponent();

            var bitness = Environment.Is64BitProcess ? "x64" : "x86";
            Text = "SOS - " + bitness;
            WindowState = FormWindowState.Maximized;
            foreach (var dir in DocumentDirectories)
            {
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            }

            Load += BrowserInterface_Load;
            //Only perform layout when control has completly finished resizing
            ResizeBegin += (s, e) => SuspendLayout();
            ResizeEnd += (s, e) => ResumeLayout(true);
            this.multiThreadedMessageLoopEnabled = multiThreadedMessageLoopEnabled;
        }

        private void BrowserInterface_Load(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = 400;
            FileInfo pdfFile = new FileInfo($"{Environment.CurrentDirectory}/Documentos/MPO/AO-AJ.SE.UHAT.pdf");
            AddTab($"{pdfFile.FullName}#page=5");
            LoadBookmarks();
            LoadDefaultTreeview();
            //UpdateStart();
        }

        #region Search methods

        //private void SearchBookmarksDataGrid()
        //{
        //    Stopwatch stopWatch = new Stopwatch();
        //    stopWatch.Start();
        //    dataGrid1.SuspendLayout();
        //    DataTable dtDocs = new DataTable();
        //    dtDocs.Columns.Add("Documento", typeof(string));
        //    DataTable dtDocsIndex = new DataTable();
        //    dtDocsIndex.Columns.Add("Documento", typeof(string));
        //    dtDocsIndex.Columns.Add("Índice", typeof(string));
        //    dtDocsIndex.Columns.Add("Página", typeof(string));
        //    string searchText = $"{txtBookmarkSearch.Text.Trim()}";
        //    IEnumerable<string> keyWords = RemoveDiacriticsAndSpecialCharactersToLower(searchText).Split(' ').Where(w => !string.IsNullOrWhiteSpace(w));
        //    Parallel.ForEach(localBookmarks, doc =>
        //    {
        //        bool hasMatch = false;
        //        foreach (var bookmark in doc.Bookmarks)
        //        {
        //            int matchCheck = 0;
        //            string title = RemoveDiacriticsAndSpecialCharactersToLower(bookmark.Title);
        //            foreach (var key in keyWords)
        //            {
        //                if (title.IndexOf(key) > 0) matchCheck++;
        //            }
        //            if (matchCheck == keyWords.Count())
        //            {
        //                string page = bookmark.Page.Split(' ')[0];
        //                title = bookmark.Title.Length > 100 ? $"{bookmark.Title.Substring(0, 100)}..." : bookmark.Title;
        //                dtDocsIndex.Rows.Add(doc.MpoCodigo, title, page);
        //                hasMatch = true;
        //            }
        //        }
        //        if (hasMatch) dtDocs.Rows.Add(doc.MpoCodigo);
        //    });
        //    int results = dtDocsIndex.Rows.Count;
        //    if (dtDocsIndex.Rows.Count == 0)
        //    {
        //        dtDocs.Rows.Add($"Sua pesquisa - {txtBookmarkSearch.Text} - não encontrou nenhum documento correspondente.");
        //        dtDocs.Rows.Add($"Sugestões:");
        //        dtDocs.Rows.Add("Certifique-se de que todas as palavras estejam escritas corretamente");
        //        dtDocs.Rows.Add("Tente palavras-chave diferentes");
        //        dtDocs.Rows.Add("Tente palavras-chave mais genéricas");
        //    }
        //    DataSet dsDataset = new DataSet();
        //    dsDataset.Tables.Add(dtDocs);
        //    dsDataset.Tables.Add(dtDocsIndex);
        //    DataRelation Datatablerelation = new DataRelation("Resultados encontrados", dsDataset.Tables[0].Columns[0], dsDataset.Tables[1].Columns[0], true);
        //    dsDataset.Relations.Add(Datatablerelation);
        //    dataGrid1.DataSource = dsDataset.Tables[0];
        //    stopWatch.Stop();
        //    treeviewStatusLabel.Text = String.Format("{0} resultados em {1:0.00} segundos", results, stopWatch.Elapsed.TotalSeconds);
        //    dataGrid1.ResumeLayout(true);
        //}

        private void LoadDefaultTreeview()
        {
            treeViewSearch.BeginUpdate();
            DirectoryInfo docDir = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "Documentos"));
            if (docDir.Exists)
            {
                ListDirectory(treeViewSearch, docDir.FullName);
                treeviewStatusLabel.Text = $"Estão vigentes {treeViewSearch.GetNodeCount(true)} diagramas e documentos";
            }
            else
            {
                SearchBookmarkPanelEnabled(false);
            }
            treeViewSearch.EndUpdate();
        }

        private void ListDirectory(TreeView treeView, string path)
        {
            treeView.Nodes.Clear();
            var rootDirectories = new DirectoryInfo(path).GetDirectories();
            foreach (var directory in rootDirectories)
            {
                treeViewSearch.Nodes.Add(CreateDirectoryNode(directory));
            }
        }

        private static TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeNode(directoryInfo.Name);
            foreach (var directory in directoryInfo.GetDirectories())
                directoryNode.Nodes.Add(CreateDirectoryNode(directory));
            foreach (var file in directoryInfo.GetFiles())
                if (!file.Name.EndsWith(".txt") && !file.Name.EndsWith(".json")) directoryNode.Nodes.Add(new TreeNode { Text = file.Name, Tag = file.FullName });
            return directoryNode;
        }

        private HashSet<ModelSearchBookmark> LocalBookmarks;
        private void LoadBookmarks()
        {
            //PM.SE.3SP NUMERO 1239 para testes
            FileInfo jsonBookmarkFile = new FileInfo($"{Environment.CurrentDirectory}/Documentos/bookmarks.json");
            string bookmarkInfo = File.Exists(jsonBookmarkFile.FullName) ? File.ReadAllText(jsonBookmarkFile.FullName) : null;
            if (bookmarkInfo == null)
            {
                SearchBookmarkPanelEnabled(false);
            }
            else
            {
                SearchBookmarkPanelEnabled(true);
            }
            try
            {
                LocalBookmarks = JsonConvert.DeserializeObject<HashSet<ModelSearchBookmark>>(bookmarkInfo);
            }
            catch (Exception)
            {
            }
        }

        private void SearchBookmarkPanelEnabled(bool v)
        {
            treeviewStatusLabel.Text = v == true ? "" : "Aguardando conclusão da atualização";
            treeviewStatusLabel.Enabled = v;
            txtBookmarkSearch.Enabled = v;
            treeViewSearch.Enabled = v;
            btnSearch.Enabled = v;
        }

        private void SearchBookmarks()
        {
            if (LocalBookmarks == null) return;
            string searchText = $"{txtBookmarkSearch.Text.Trim()}";

            if (searchText.Length == 0)
            {
                LoadDefaultTreeview();
                return;
            }
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            treeViewSearch.BeginUpdate();
            treeViewSearch.Nodes.Clear();
            treeviewStatusLabel.Text = "Procurando...";

            if (searchText.Length > 3)
            {
                IEnumerable<string> keyWords = RemoveDiacriticsAndSpecialCharactersToLower(searchText).Split(' ').Where(w => !string.IsNullOrWhiteSpace(w));
                var treeNodeResults = new List<TreeNode>();
                Parallel.ForEach(LocalBookmarks, doc =>
                {
                    var docNode = new TreeNode { Text = doc.MpoCodigo };
                    foreach (var bookmark in doc.Bookmarks)
                    {
                        int matchCheck = 0;
                        string title = RemoveDiacriticsAndSpecialCharactersToLower(bookmark.Title);
                        foreach (var key in keyWords)
                        {
                            if (title.IndexOf(key) > -1) matchCheck++;
                        }
                        if (matchCheck == keyWords.Count())
                        {
                            title = bookmark.Title.Length > 70 ? $"{bookmark.Title.Substring(0, 70)}..." : bookmark.Title;
                            TreeNode treenode = new TreeNode { Text = $"{title}", Tag = $"{bookmark.PathAndPage}", ToolTipText = bookmark.Title };
                            docNode.Nodes.Add(treenode);
                        }
                    }                   
                    if (docNode.Nodes.Count > 0) lock (treeNodeResults)treeNodeResults.Add(docNode);                
                });
                if (treeNodeResults.Count > 0) treeViewSearch.Nodes.AddRange(treeNodeResults.ToArray());
            }
            int results = treeViewSearch.GetNodeCount(true);
            if (treeViewSearch.Nodes.Count == 0)
            {
                treeViewSearch.Nodes.Add($"Sua pesquisa - {txtBookmarkSearch.Text} - não encontrou nenhum documento correspondente.");
                treeViewSearch.Nodes.Add($"Sugestões:");
                var suggestionNodes = new TreeNode[] { new TreeNode("Certifique-se de que todas as palavras estejam escritas corretamente"), new TreeNode("Tente palavras-chave diferentes"), new TreeNode("Tente palavras-chave mais genéricas") };
                treeViewSearch.Nodes[1].Nodes.AddRange(suggestionNodes);
                treeViewSearch.ExpandAll();
            }
            treeViewSearch.EndUpdate();
            stopWatch.Stop();
            treeviewStatusLabel.Text = String.Format("{0} resultados em {1:0.00} segundos", results, stopWatch.Elapsed.TotalSeconds);
        }
        private void SearchBookmarksButtonClick(object sender, EventArgs e)
        {
            SearchBookmarks();
        }
        private void TxtBookmarkSearchKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }
            SearchBookmarks();
        }

        private void TreeViewSearchNodeClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag != null && e.Button == MouseButtons.Right)
            {
                treeViewSearch.SelectedNode = e.Node;
                AddTab(e.Node.Tag.ToString());
            }
            if (e.Node.Tag != null && e.Button == MouseButtons.Left)
            {
                treeViewSearch.SelectedNode = e.Node;
                LoadActualTab(e.Node.Tag.ToString());
            }
        }
        private string RemoveDiacriticsAndSpecialCharactersToLower(string text)
        {
            byte[] tempBytes = Encoding.GetEncoding("ISO-8859-8").GetBytes(text.ToLower());
            string str = Encoding.UTF8.GetString(tempBytes);
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append(' ');

                }
            }
            return sb.ToString();
        }

        private string TreenodeTextToFit(string nodeText, int length)
        {
            //treeview não aceita newline; tive problema para definir os bounds do texto após o fit
            //TODO drawmode treeview para multiple line
            IList<string> listNodeText = new List<string>();
            for (var i = 0; i < nodeText.Length; i += 60)
            {
                listNodeText.Add(nodeText.Substring(i, Math.Min(60, nodeText.Length - i)));
            }
            return String.Join(Environment.NewLine, listNodeText);
        }

        private void TreeViewSearchNodeDraw(object sender, DrawTreeNodeEventArgs e)
        {

            if (e.Node == null) return;

            // if treeview's HideSelection property is "True", 
            // this will always returns "False" on unfocused treeview
            var selected = (e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected;
            var unfocused = !e.Node.TreeView.Focused;
            Font font = new Font(e.Node.NodeFont ?? e.Node.TreeView.Font, FontStyle.Underline);
            //Font fontPage = new Font(e.Node.NodeFont.FontFamily, e.Node.NodeFont.Size -2, FontStyle.Underline);

            // we need to do owner drawing only on a selected node
            // and when the treeview is unfocused, else let the OS do it for us
            if (selected && unfocused)
            {
                e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
                TextRenderer.DrawText(e.Graphics, e.Node.Text, font, e.Bounds, SystemColors.HighlightText, TextFormatFlags.GlyphOverhangPadding);
            }
            else if (e.State == TreeNodeStates.Hot)
            {
                //foreColor = Color.Red;
                TextRenderer.DrawText(e.Graphics, e.Node.Text, font, e.Bounds, Color.Blue, Color.White, TextFormatFlags.GlyphOverhangPadding);
            }
            else
            {
                e.DrawDefault = true;
            }

            //if (e.Node.Tag != null) //adicionar número da página
            //{
            //    //TextRenderer.DrawText(e.Graphics, e.Node.Tag.ToString(), font, e.Bounds, Color.Blue, Color.White, TextFormatFlags.GlyphOverhangPadding);

            //    e.Graphics.DrawString(e.Node.Tag.ToString(), font, Brushes.Black, e.Bounds.Right, e.Bounds.Top);
            //}
        }
        #endregion

        #region Update methods
        private void UpdateStart()
        {
            WebScrap webScrap = new WebScrap(statusOutputLinkLabel);
            updateStartToolStripMenuItem.Enabled = false;
            updateStatusToolStripMenuItem.Checked = true;
            statusOutputLinkLabel.Visible = true;
            statusOutputLinkLabel.Enabled = false;
            statusOutputLinkLabel.Links.Clear();

            Task.Run(() =>
            {
                try
                {
                    webScrap.UpdateDocuments();
                    statusOutputLinkLabel.InvokeOnUiThreadIfRequired(() =>
                    {
                        statusOutputLinkLabel.Links.Add("MM/dd/yyyy HH:mm:ss: Atualização concluída. Cheque o histórico de atualização clicando ".Length, "aqui".Length, Path.Combine(Environment.CurrentDirectory, "Documentos", "log.txt"));
                        statusOutputLinkLabel.Text = $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}: Atualização concluída. Cheque o histórico de atualização clicando aqui";
                        statusOutputLinkLabel.Enabled = true;
                        updateStartToolStripMenuItem.Enabled = true;
                        LoadBookmarks();
                        LoadDefaultTreeview();
                    });
                }
                catch (Exception ex)
                {
                    statusOutputLinkLabel.InvokeOnUiThreadIfRequired(() => statusOutputLinkLabel.Text = $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}: Não foi possível concluir a atualização de documentos. Cheque se está conectado à Intranet e Internet. Caso o problema persista comunique o administrador da aplicação");
                    File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "Documentos", "crash_report.txt"), $"Data: {ex.Data}{Environment.NewLine}Source:{ex.Source}{Environment.NewLine}StackTrace:{ex.StackTrace}{Environment.NewLine}TargetSite:{ex.TargetSite}{Environment.NewLine}InnerExceptionMessage:{ex.InnerException.Message}{Environment.NewLine}ExceptionMessage:{ex.Message}{Environment.NewLine}");
                }
            });

        }
        private void StatusOutputLinkLabelClick(object sender, LinkLabelLinkClickedEventArgs e)
        {
            statusOutputLinkLabel.LinkVisited = true;
            AddTab(e.Link.LinkData.ToString());
        }

        #endregion

        #region BrowserTabControl methods
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

        private void BrowserTabDrawItem(object sender, DrawItemEventArgs e)
        {
            browserTabControl.SuspendLayout();
            try
            {
                if (e.Index != browserTabControl.SelectedIndex)
                    e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), e.Bounds);
                else
                    e.Graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds);

                if (e.Index == browserTabControl.TabCount - 1)
                {
                    e.Graphics.DrawString("+", new Font("Arial", 8, FontStyle.Bold), Brushes.Black, e.Bounds.Right - 25, e.Bounds.Top + 4);
                    browserTabControl.TabPages[e.Index].Width = 18;
                }
                else
                {
                    e.Graphics.DrawString("x", new Font("Arial", 8, FontStyle.Bold), Brushes.Black, e.Bounds.Right - 15, e.Bounds.Top + 4);
                    e.Graphics.DrawString(browserTabControl.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + 5, e.Bounds.Top + 4);
                }
                e.DrawFocusRectangle();
            }
            catch (Exception)
            {
                return;
            }
            browserTabControl.ResumeLayout(true);
        }

        private void BrowserTabMouseClick(object sender, MouseEventArgs e)
        {
            TabControl tc = (TabControl)sender;
            Point p = e.Location;
            Point _imageLocation = new Point(13, 5);
            Point _imgHitArea = new Point(13, 2);
            int _tabWidth = 0;
            _tabWidth = browserTabControl.GetTabRect(tc.SelectedIndex).Width - (_imgHitArea.X);
            Rectangle r = browserTabControl.GetTabRect(tc.SelectedIndex);
            r.Offset(_tabWidth, _imgHitArea.Y);
            r.Width = 16;
            r.Height = 16;
            if (r.Contains(p))
            {
                RemoveSelectedTab();
            }
        }


        private void BrowserTabSelectedIndexChanged(object sender, EventArgs e)
        {
            if (browserTabControl.SelectedIndex == browserTabControl.TabCount - 1)
            {
                AddTab(DefaultUrlForAddedTabs);
            }
        }
        private void LoadActualTab(string url)
        {
            treeViewSearch.Enabled = false;
            browserTabControl.SuspendLayout();
            var tabPage = browserTabControl.Controls[browserTabControl.SelectedIndex];
            var control = tabPage.Controls[0] as BrowserTabUserControl;
            tabPage.Controls.Remove(control);
            control.Dispose();//necessário para forçar reload sem cache para url parameters

            var browser = new BrowserTabUserControl(AddTab, url, multiThreadedMessageLoopEnabled)
            {
                Dock = DockStyle.Fill,
            };

            //This call isn't required for the sample to work. 
            //It's sole purpose is to demonstrate that #553 has been resolved.
            browser.CreateControl();

            tabPage.Controls.Add(browser);

            browserTabControl.ResumeLayout(true);
            treeViewSearch.Enabled = true;
        }

        public void AddTab(string url, int? insertIndex = null)
        {
            browserTabControl.SuspendLayout();

            var browser = new BrowserTabUserControl(AddTab, url, multiThreadedMessageLoopEnabled)
            {
                Dock = DockStyle.Fill,
            };

            var tabPage = new TabPage(url)
            {
                Dock = DockStyle.Fill,
                Text = "Carregando...".PadRight(30)
            };

            //This call isn't required for the sample to work. 
            //It's sole purpose is to demonstrate that #553 has been resolved.
            browser.CreateControl();

            tabPage.Controls.Add(browser);

            if (insertIndex == null)
            {
                //browserTabControl.TabPages.Add(tabPage);
                browserTabControl.TabPages.Insert(browserTabControl.TabCount - 1, tabPage);

            }
            else
            {
                browserTabControl.TabPages.Insert(insertIndex.Value, tabPage);
            }

            //Make newly created tab active
            browserTabControl.SelectedTab = tabPage;

            browserTabControl.ResumeLayout(true);
        }
        #endregion

        #region MenuStrip click methods
        private void VisualHistoricoItemClick(object sender, EventArgs e)
        {
            AddTab(Path.Combine(Environment.CurrentDirectory, "Documentos", "log.txt"));
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
        //public void RemoveTab(IntPtr windowHandle)
        //{
        //    var parentControl = FromChildHandle(windowHandle);
        //    if (!parentControl.IsDisposed)
        //    {
        //        if (parentControl.Parent is TabPage tabPage)
        //        {
        //            browserTabControl.TabPages.Remove(tabPage);
        //        }
        //        else if (parentControl.Parent is Panel panel)
        //        {
        //            var browserTabUserControl = (BrowserTabUserControl)panel.Parent;

        //            var tab = (TabPage)browserTabUserControl.Parent;
        //            browserTabControl.TabPages.Remove(tab);
        //        }
        //    }
        //}
        private void UpdateStartItemClick(object sender, EventArgs e)
        {
            UpdateStart();
        }

        private void UpdateStatusItemClick(object sender, EventArgs e)
        {
            updateStatusToolStripMenuItem.Checked = !statusOutputLinkLabel.Visible;
            statusOutputLinkLabel.Visible = !statusOutputLinkLabel.Visible;
        }
        private void DownloadMessagesItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                exibirMensagensDeDownloadToolStripMenuItem.Checked = !control.downloadOutputLabel.Visible;
                control.downloadOutputLabel.Visible = !control.downloadOutputLabel.Visible;
            }
        }
        private void DisplayOutputMessagesItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.DisplayOutputMessages();
                exibirMsgConsoleToolStripMenuItem.Checked = !exibirMsgConsoleToolStripMenuItem.Checked;
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

        //private void CopySourceToClipBoardAsyncClick(object sender, EventArgs e)
        //{
        //    var control = GetCurrentTabControl();
        //    if (control != null)
        //    {
        //        control.CopySourceToClipBoardAsync();
        //    }
        //}

        private void NewTabToolStripMenuItemClick(object sender, EventArgs e)
        {
            AddTab(DefaultUrlForAddedTabs);
        }

        private void RemoveSelectedTab()
        {
            if (browserTabControl.TabPages.Count == 2)
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

            browserTabControl.SelectedIndex = currentIndex > 1 ? currentIndex - 1 : 0;

            if (browserTabControl.TabPages.Count == 0)
            {
                ExitApplication();
            }
        }

        private void CloseTabToolStripMenuItemClick(object sender, EventArgs e)
        {
            RemoveSelectedTab();
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

        //private void CloseDevToolsMenuItemClick(object sender, EventArgs e)
        //{
        //    var control = GetCurrentTabControl();
        //    if (control != null)
        //    {
        //        control.Browser.CloseDevTools();
        //    }
        //}

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

        //private void CurrentZoomLevelToolStripMenuItemClick(object sender, EventArgs e)
        //{
        //    var control = GetCurrentTabControl();
        //    if (control != null)
        //    {
        //        var task = control.Browser.GetZoomLevelAsync();
        //        task.ContinueWith(previous =>
        //        {
        //            if (previous.Status == TaskStatus.RanToCompletion)
        //            {
        //                var currentLevel = previous.Result;
        //                MessageBox.Show("Current ZoomLevel: " + currentLevel.ToString());
        //            }
        //            else
        //            {
        //                MessageBox.Show("Unexpected failure of calling CEF->GetZoomLevelAsync: " + previous.Exception.ToString());
        //            }
        //        }, TaskContinuationOptions.HideScheduler);
        //    }
        //}

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
                        MessageBox.Show("PDF salvo em " + dialog.FileName);
                    }
                    else
                    {
                        MessageBox.Show("Não foi possível salvar o PDF, cheque se você tem permissão de escrita para " + dialog.FileName);
                    }

                }

            }
        }
        #endregion
    }
}
