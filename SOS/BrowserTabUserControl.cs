﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp.WinForms;
using CefSharp;
using System.Runtime.InteropServices;
using System.Threading;
using SOS.Handlers;
using CefSharp.WinForms.Internals;
using System.IO;
using Newtonsoft.Json;

namespace SOS
{
    public partial class BrowserTabUserControl : UserControl
    {
        public IWinFormsWebBrowser Browser { get; private set; }
        private IntPtr browserHandle;
        private ChromeWidgetMessageInterceptor messageInterceptor;
        private bool multiThreadedMessageLoopEnabled;
        //public BrowserTabUserControl(Action<string, int?, string> openNewTab, string url, bool multiThreadedMessageLoopEnabled)
        public BrowserTabUserControl(string url, bool multiThreadedMessageLoopEnabled)
        {
            InitializeComponent();
           
            var browser = new ChromiumWebBrowser(url)
            {
                Dock = DockStyle.Fill
            };
            
            browserPanel.Controls.Add(browser);

            Browser = browser;

            browser.MenuHandler = new MenuHandler(this);
            //browser.RequestHandler = new WinFormsRequestHandler(openNewTab);
            //browser.JsDialogHandler = new JsDialogHandler();
            browser.DownloadHandler = new DownloadHandler(this);
            
            if (multiThreadedMessageLoopEnabled)
            {
                browser.KeyboardHandler = new KeyboardHandler();
            }
            else
            {
                //When MultiThreadedMessageLoop is disabled we don't need the
                //CefSharp focus handler implementation.
                browser.FocusHandler = null;
            }
            //browser.LifeSpanHandler = new LifeSpanHandler();
            browser.LoadingStateChanged += OnBrowserLoadingStateChanged;
           
            browser.TitleChanged += OnBrowserTitleChanged;
            browser.AddressChanged += OnBrowserAddressChanged;
            browser.StatusMessage += OnBrowserStatusMessage;
            browser.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;

            //displayOutput messages
            outputLabel.Visible = false;
            var version = String.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}", Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion);
            DisplayOutput(version);
            browser.ConsoleMessage += OnBrowserConsoleMessage;
            browser.LoadError += OnLoadError;

            //browser.JavascriptObjectRepository.Register("bound", new BoundObject(), isAsync: false, options: BindingOptions.DefaultBinder);
            //browser.JavascriptObjectRepository.Register("boundAsync", new AsyncBoundObject(), isAsync: true, options: BindingOptions.DefaultBinder);

            //If you call CefSharp.BindObjectAsync in javascript and pass in the name of an object which is not yet
            //bound, then ResolveObject will be called, you can then register it
            //browser.JavascriptObjectRepository.ResolveObject += (sender, e) =>
            //{
            //    var repo = e.ObjectRepository;
            //    if (e.ObjectName == "boundAsync2")
            //    {
            //        repo.Register("boundAsync2", new AsyncBoundObject(), isAsync: true, options: BindingOptions.DefaultBinder);
            //    }
            //};

            browser.RenderProcessMessageHandler = new RenderProcessMessageHandler();
            browser.DisplayHandler = new DisplayHandler();
            browser.FindHandler = new FindHandler(this);
            //browser.MouseDown += OnBrowserMouseClick;
            browser.HandleCreated += OnBrowserHandleCreated;
            //browser.ResourceHandlerFactory = new FlashResourceHandlerFactory();
            this.multiThreadedMessageLoopEnabled = multiThreadedMessageLoopEnabled;

            //var eventObject = new ScriptedMethodsBoundObject();
            //eventObject.EventArrived += OnJavascriptEventArrived;
            //// Use the default of camelCaseJavascriptNames
            //// .Net methods starting with a capitol will be translated to starting with a lower case letter when called from js
            //browser.JavascriptObjectRepository.Register("boundEvent", eventObject, isAsync: false, options: BindingOptions.DefaultBinder);

            //CefExample.RegisterTestResources(browser);    
        }

        private void ShowRelatedDocuments(string url)
        {
            if (url.IndexOf(".pdf") == -1) return;
            if (url.IndexOf("/MPO/") > -1)
            {
                BrowserInterface form = ParentForm as BrowserInterface;
                string relatedDocs = form.LocalRefsMOP.Where(w => w.MpoCodigo == Path.GetFileNameWithoutExtension(url)).Select(s => s.MpoAlteradosPelasMops).FirstOrDefault();
                if (string.IsNullOrEmpty(relatedDocs)) return;
                var chrome = browserPanel.Controls[0] as ChromiumWebBrowser;
                string links = "";
                foreach (var doc in relatedDocs.Split(','))
                {
                    links += $"<a href=\"MOP/{doc.Replace('/', '-')}.pdf\">{doc}</a>, ";
                }
                string script = @"
                var embedNode = document.getElementsByTagName('embed')[0]
                var node = document.createElement('div');    
                node.innerHTML = 'Documentos relacionados: " + links + @"'
                document.getElementsByTagName('body')[0].insertBefore(node, embedNode)
                document.body.style.backgroundColor = 'rgb(255, 255, 255)';";
                chrome.ExecuteScriptAsyncWhenPageLoaded(script);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }

                if (messageInterceptor != null)
                {
                    messageInterceptor.ReleaseHandle();
                    messageInterceptor = null;
                }
            }
            base.Dispose(disposing);
        }

        #region browser methods
        //private static void OnJavascriptEventArrived(string eventName, object eventData)
        //{
        //    switch (eventName)
        //    {
        //        case "click":
        //            {
        //                var message = eventData.ToString();
        //                var dataDictionary = eventData as Dictionary<string, object>;
        //                if (dataDictionary != null)
        //                {
        //                    var result = string.Join(", ", dataDictionary.Select(pair => pair.Key + "=" + pair.Value));
        //                    message = "event data: " + result;
        //                }
        //                MessageBox.Show(message, "Javascript event arrived", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                break;
        //            }
        //    }
        //}

        //private void OnBrowserMouseClick(object sender, MouseEventArgs e)
        //{
        //    MessageBox.Show("Mouse Clicked" + e.X + ";" + e.Y + ";" + e.Button);
        //}

        private void OnBrowserHandleCreated(object sender, EventArgs e)
        {
            browserHandle = ((ChromiumWebBrowser)Browser).Handle;
        }

        private void OnLoadError(object sender, LoadErrorEventArgs args)
        {
            DisplayOutput("Load Error:" + args.ErrorCode + ";" + args.ErrorText);
        }

        private void OnBrowserConsoleMessage(object sender, ConsoleMessageEventArgs args)
        {
            DisplayOutput(string.Format("Line: {0}, Source: {1}, Message: {2}", args.Line, args.Source, args.Message));
        }

        private void OnBrowserStatusMessage(object sender, StatusMessageEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => statusLabel.Text = args.Value);
        }

        private void OnBrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
        {
            SetCanGoBack(args.CanGoBack);
            SetCanGoForward(args.CanGoForward);
            this.InvokeOnUiThreadIfRequired(() => SetIsLoading(args.IsLoading));
        }

        private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => {
                TabPage currentTabPage = Parent as TabPage;
                currentTabPage.Text = args.Title.Length > 20 ? $"{args.Title.Substring(0, 20)}...".PadRight(30) : args.Title.PadRight(30); //6 espaços para o (x) da tab
                currentTabPage.ToolTipText = args.Title;
            });
        }

        private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => urlTextBox.Text = args.Address);
            ShowRelatedDocuments(args.Address);
        }

        private void SetCanGoBack(bool canGoBack)
        {
            this.InvokeOnUiThreadIfRequired(() => backButton.Enabled = canGoBack);
        }

        private void SetCanGoForward(bool canGoForward)
        {
            this.InvokeOnUiThreadIfRequired(() => forwardButton.Enabled = canGoForward);
        }

        private void SetIsLoading(bool isLoading)
        {
            goButton.Text = isLoading ?
                "Parar" :
                "Ir";
            goButton.Image = isLoading ?
                Properties.Resources.times_circle_regular :
                Properties.Resources.redo_alt_solid;

            HandleToolStripLayout();
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private void OnIsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs args)
        {
            if (args.IsBrowserInitialized)
            {
                //Get the underlying browser host wrapper
                var browserHost = Browser.GetBrowser().GetHost();
                var requestContext = browserHost.RequestContext;
                string errorMessage;
                // Browser must be initialized before getting/setting preferences
                var success = requestContext.SetPreference("enable_do_not_track", true, out errorMessage);
                if (!success)
                {
                    this.InvokeOnUiThreadIfRequired(() => MessageBox.Show("Unable to set preference enable_do_not_track errorMessage: " + errorMessage));
                }

                //Example of disable spellchecking
                //success = requestContext.SetPreference("browser.enable_spellchecking", false, out errorMessage);

                var preferences = requestContext.GetAllPreferences(true);
                var doNotTrack = (bool)preferences["enable_do_not_track"];

                //Use this to check that settings preferences are working in your code
                //success = requestContext.SetPreference("webkit.webprefs.minimum_font_size", 24, out errorMessage);

                //If we're using CefSetting.MultiThreadedMessageLoop (the default) then to hook the message pump,
                // which running in a different thread we have to use a NativeWindow
                if (multiThreadedMessageLoopEnabled)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            while (true)
                            {
                                IntPtr chromeWidgetHostHandle;
                                if (ChromeWidgetHandleFinder.TryFindHandle(browserHandle, out chromeWidgetHostHandle))
                                {
                                    messageInterceptor = new ChromeWidgetMessageInterceptor((Control)Browser, chromeWidgetHostHandle, message =>
                                    {
                                        const int WM_MOUSEACTIVATE = 0x0021;
                                        const int WM_NCLBUTTONDOWN = 0x00A1;
                                        const int WM_LBUTTONDOWN = 0x0201;

                                        if (message.Msg == WM_MOUSEACTIVATE)
                                        {
                                            // The default processing of WM_MOUSEACTIVATE results in MA_NOACTIVATE,
                                            // and the subsequent mouse click is eaten by Chrome.
                                            // This means any .NET ToolStrip or ContextMenuStrip does not get closed.
                                            // By posting a WM_NCLBUTTONDOWN message to a harmless co-ordinate of the
                                            // top-level window, we rely on the ToolStripManager's message handling
                                            // to close any open dropdowns:
                                            // http://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/ToolStripManager.cs,1249
                                            var topLevelWindowHandle = message.WParam;
                                            PostMessage(topLevelWindowHandle, WM_NCLBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
                                        }
                                        //Forward mouse button down message to browser control
                                        //else if(message.Msg == WM_LBUTTONDOWN)
                                        //{
                                        //    PostMessage(browserHandle, WM_LBUTTONDOWN, message.WParam, message.LParam);
                                        //}

                                        // The ChromiumWebBrowserControl does not fire MouseEnter/Move/Leave events, because Chromium handles these.
                                        // However we can hook into Chromium's messaging window to receive the events.
                                        //
                                        //const int WM_MOUSEMOVE = 0x0200;
                                        //const int WM_MOUSELEAVE = 0x02A3;
                                        //
                                        //switch (message.Msg) {
                                        //    case WM_MOUSEMOVE:
                                        //        Console.WriteLine("WM_MOUSEMOVE");
                                        //        break;
                                        //    case WM_MOUSELEAVE:
                                        //        Console.WriteLine("WM_MOUSELEAVE");
                                        //        break;
                                        //}
                                    });

                                    break;
                                }
                                else
                                {
                                    // Chrome hasn't yet set up its message-loop window.
                                    Thread.Sleep(10);
                                }
                            }
                        }
                        catch
                        {
                            // Errors are likely to occur if browser is disposed, and no good way to check from another thread
                        }
                    });
                }
            }
        }
        #endregion

        #region Form controls
        private void DisplayOutput(string output)
        {
            this.InvokeOnUiThreadIfRequired(() => outputLabel.Text = output);
        }

        private void HandleToolStripLayout(object sender, LayoutEventArgs e)
        {
            HandleToolStripLayout();
        }

        private void HandleToolStripLayout()
        {
            var width = toolStrip1.Width;
            foreach (ToolStripItem item in toolStrip1.Items)
            {
                if (item != urlTextBox)
                {
                    width -= item.Width - item.Margin.Horizontal;
                }
            }
            urlTextBox.Width = Math.Max(0, width - urlTextBox.Margin.Horizontal - 18);
        }
        private void DownloadLinkLabelClick(object sender, LinkLabelLinkClickedEventArgs e)
        {
            downloadOutputLabel.LinkVisited = true;
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void GoButtonClick(object sender, EventArgs e)
        {
            LoadUrl(urlTextBox.Text);
        }

        private void BackButtonClick(object sender, EventArgs e)
        {
            Browser.Back();
        }

        private void ForwardButtonClick(object sender, EventArgs e)
        {
            Browser.Forward();
        }

        private void UrlTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            LoadUrl(urlTextBox.Text);
        }

        private void LoadUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                Browser.Load(url);
            }
        }

        //public async void CopySourceToClipBoardAsync()
        //{
        //    var htmlSource = await Browser.GetSourceAsync();

        //    Clipboard.SetText(htmlSource);
        //    DisplayOutput("HTML Source copied to clipboard");
        //}

        private void ToggleBottomToolStrip()
        {
            if (toolStrip2.Visible)
            {
                Browser.StopFinding(true);
                toolStrip2.Visible = false;
            }
            else
            {
                toolStrip2.Visible = true;
                findTextBox.Focus();
            }
        }

        private void FindNextButtonClick(object sender, EventArgs e)
        {
            if (findTextLabel.Text.Split('/').First() == findTextLabel.Text.Split('/').Last())
            {
                Find(false);
            }
            else
            {
                Find(true);
            }
        }

        private void FindPreviousButtonClick(object sender, EventArgs e)
        {
            if (findTextLabel.Text.Split('/').First() == "1")
            {
                Find(true);
            }
            else
            {
                Find(false);
            }
        }

        private void Find(bool next)
        {
            if (!string.IsNullOrEmpty(findTextBox.Text))
            {
                Browser.Find(0, findTextBox.Text, next, false, false);
            }
        }

        private void FindTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }
            if(findTextLabel.Text.Split('/').First() == findTextLabel.Text.Split('/').Last())
            {
                Find(false);
            }
            else
            {
                Find(true);
            }
        }

        public void ShowFind()
        {
            ToggleBottomToolStrip();
        }

        public void DisplayOutputMessages()
        {
            if (outputLabel.Visible)
            {
                outputLabel.Visible = false;
            }
            else
            {
                outputLabel.Visible = true;
            }
        }

        private void FindCloseButtonClick(object sender, EventArgs e)
        {
            ToggleBottomToolStrip();
        }
        #endregion
    }
}
