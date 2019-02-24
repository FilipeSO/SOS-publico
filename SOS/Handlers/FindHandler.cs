using CefSharp;
using CefSharp.Structs;
using CefSharp.WinForms.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOS.Handlers
{
    class FindHandler : IFindHandler
    {
        private BrowserTabUserControl currentBrowserTab = null;
        private string currentSearch = "";
        private int countFix = 0;
        public FindHandler(BrowserTabUserControl form)
        {
            currentBrowserTab = form;
        }
        public void OnFindResult(IWebBrowser chromiumWebBrowser, IBrowser browser, int identifier, int count, Rect selectionRect, int activeMatchOrdinal, bool finalUpdate)
        {
            if (finalUpdate)
            {
                if (currentSearch != currentBrowserTab.findTextBox.Text)
                {
                    countFix = count;
                    currentSearch = currentBrowserTab.findTextBox.Text;
                }
                //activeMatchOrdinal = activeMatchOrdinal > countFix ? 1 : activeMatchOrdinal; //estouro da contagem inevitavel em PDF devido limitação cefsharp; estouro impedido por bloqueio na interface
                //currentBrowserTab.InvokeOnUiThreadIfRequired(() => currentBrowserTab.findTextLabel.Text = $"identifier:{identifier}; count:{count}; selectionReact:x>{selectionRect.X},y>{selectionRect.Y}, width>{selectionRect.Width}, height>{selectionRect.Height}; activeMatchOrdinal:{activeMatchOrdinal}; finalUpdate:{finalUpdate}; solução: {activeMatchOrdinal}/{countFix}");
                currentBrowserTab.InvokeOnUiThreadIfRequired(() => currentBrowserTab.findTextLabel.Text = $"{activeMatchOrdinal}/{countFix}");
            }

        }
    }
}
