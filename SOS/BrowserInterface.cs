using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Internals;
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
            AddTab($"{pdfFile.FullName}#page=5");
            UpdateStart();
        }
        
        private void UpdateStart()
        {
            WebScrap webScrap = new WebScrap(statusOutputLinkLabel);
            updateStartToolStripMenuItem.Enabled = false;
            updateStatusToolStripMenuItem.Checked = true;
            statusOutputLinkLabel.Visible = true;
            statusOutputLinkLabel.Enabled = false;
            statusOutputLinkLabel.Links.Clear();
            try
            {
                Task.Run(() =>
                {
                    webScrap.UpdateDocuments();
                    statusOutputLinkLabel.InvokeOnUiThreadIfRequired(() => {
                        statusOutputLinkLabel.Links.Add("MM/dd/yyyy HH:mm:ss: Atualização concluída. Cheque o histórico de atualização clicando ".Length, "aqui".Length, $"{Environment.CurrentDirectory}/Documentos/log.txt");
                        statusOutputLinkLabel.Text = $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}: Atualização concluída. Cheque o histórico de atualização clicando aqui";
                        statusOutputLinkLabel.Enabled = true;
                        updateStartToolStripMenuItem.Enabled = true;
                    });
                });
            }
            catch (Exception ex)
            {
                statusOutputLinkLabel.InvokeOnUiThreadIfRequired(() => statusOutputLinkLabel.Text = $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}: Não foi possível concluir a atualização de documentos. Cheque se está conectado à Intranet e Internet. Caso o problema persista comunique o administrador da aplicação");
                File.WriteAllText($"{Environment.CurrentDirectory}/Documentos/crash_report.txt", $"Data: {ex.Data}{Environment.NewLine}Source:{ex.Source}{Environment.NewLine}StackTrace:{ex.StackTrace}{Environment.NewLine}TargetSite:{ex.TargetSite}{Environment.NewLine}InnerExceptionMessage:{ex.InnerException.Message}{Environment.NewLine}ExceptionMessage{ex.Message}{Environment.NewLine}{Environment.NewLine}");
            }
        }

        private void StatusOutputLinkLabelClick(object sender, LinkLabelLinkClickedEventArgs e)
        {
            statusOutputLinkLabel.LinkVisited = true;
            AddTab(e.Link.LinkData.ToString());
        }

        private void SearchBookmarksButtonClick(object sender, EventArgs e)
        {
            treeViewSearch.BeginUpdate();
            treeViewSearch.Nodes.Clear();
            //PM.SE.3SP NUMERO 1239 para testes
            FileInfo jsonBookmarkFile = new FileInfo($"{Environment.CurrentDirectory}/Documentos/MPO/bookmarks.json");
            string bookmarkInfo = File.Exists(jsonBookmarkFile.FullName) ? File.ReadAllText(jsonBookmarkFile.FullName) : null;
            var localBookmarks = string.IsNullOrEmpty(bookmarkInfo) ? new List<ModelSearchBookmark>() : JsonConvert.DeserializeObject<IEnumerable<ModelSearchBookmark>>(bookmarkInfo);
            localBookmarks = localBookmarks.Where(w => w.Bookmarks != null);
            string searchText = $"{txtBookmarkSearch.Text.Trim()}"; 
            IEnumerable<string> keyWords = RemoveDiacriticsAndSpecialCharactersToLower(searchText).Split(' ').Where(w => !string.IsNullOrWhiteSpace(w));
            foreach (var doc in localBookmarks)
            {
                var docNode = new TreeNode { Text = doc.MpoCodigo };
                foreach (var bookmark in doc.Bookmarks)
                {
                    string title = RemoveDiacriticsAndSpecialCharactersToLower(bookmark.Title);
                    string page = bookmark.Page.Split(' ')[0];
                    int matchCheck = 0;
                    foreach (var key in keyWords)
                    {
                        if (title.IndexOf(key) > 0) matchCheck++;
                    }
                    if (matchCheck == keyWords.Count())
                    {
                        docNode.Nodes.Add(new TreeNode { Text = $"{bookmark.Title.TrimEnd('\r', '\n', ' ')}", Tag = page });
                    }
                }
                if (docNode.Nodes.Count > 0) treeViewSearch.Nodes.Add(docNode);
            }
            if (treeViewSearch.Nodes.Count == 0)
            {
                treeViewSearch.Nodes.Add($"Sua pesquisa - {txtBookmarkSearch.Text} - não encontrou nenhum documento correspondente.");
                treeViewSearch.Nodes.Add($"Sugestões:");
                var suggestionNodes = new TreeNode[] { new TreeNode("Certifique-se de que todas as palavras estejam escritas corretamente"), new TreeNode("Tente palavras-chave diferentes"), new TreeNode("Tente palavras-chave mais genéricas") };
                treeViewSearch.Nodes[1].Nodes.AddRange(suggestionNodes);
                treeViewSearch.ExpandAll();
            }
            treeViewSearch.EndUpdate();
        }

        private void TreeViewSearchNodeClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeViewSearch.Enabled = false;
            if (e.Node.Tag != null && e.Button == MouseButtons.Right)
            {
                treeViewSearch.SelectedNode = e.Node;
                FileInfo pdfFile = new FileInfo($"{Environment.CurrentDirectory}/Documentos/MPO/{e.Node.Parent.Text}.pdf");
                AddTab($"{pdfFile.FullName}#page={e.Node.Tag}");
            }
            if (e.Node.Tag != null && e.Button == MouseButtons.Left)
            {
                treeViewSearch.SelectedNode = e.Node;
                FileInfo pdfFile = new FileInfo($"{Environment.CurrentDirectory}/Documentos/MPO/{e.Node.Parent.Text}.pdf");
                LoadActualTab($"{pdfFile.FullName}#page={e.Node.Tag}");
            }
            treeViewSearch.Enabled = true;
        }
        private string RemoveDiacriticsAndSpecialCharactersToLower(string text)
        {
            byte[]  tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(text.ToLower());
            string str = System.Text.Encoding.UTF8.GetString(tempBytes);
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

                if (e.Index == browserTabControl.TabCount-1)
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
            if(browserTabControl.SelectedIndex == browserTabControl.TabCount-1)
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
                browserTabControl.TabPages.Insert(browserTabControl.TabCount-1,tabPage);

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

            browserTabControl.SelectedIndex = currentIndex > 1 ? currentIndex -1 : 0;

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
