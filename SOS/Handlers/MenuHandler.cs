// Copyright © 2014 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using CefSharp;
using System;
using CefSharp.WinForms.Internals;

namespace SOS.Handlers
{
    internal class MenuHandler : IContextMenuHandler
    {
        private BrowserTabUserControl currentBrowserTab = null;

        public MenuHandler(BrowserTabUserControl form)
        {
            currentBrowserTab = form;
        }
        private string statusLabelLink = "";
        private string selectionTextQuery = "";
        private const int ShowDevTools = 26501;
        private const int OpenNewTab = 26502;
        private const int SearchGoogle = 26503;

        void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            if (!string.IsNullOrWhiteSpace(parameters.SelectionText))
            {
                selectionTextQuery = parameters.SelectionText;
                model.AddItem((CefMenuCommand)SearchGoogle, $"Pesquisar \"{selectionTextQuery.Substring(0, Math.Min(selectionTextQuery.Length, 60))}\" no Google");
                
            }
            statusLabelLink = currentBrowserTab.statusLabel.Text;
            if (Uri.IsWellFormedUriString(statusLabelLink, UriKind.RelativeOrAbsolute) && !string.IsNullOrWhiteSpace(statusLabelLink))
            {
                model.Clear();
                model.AddItem((CefMenuCommand)OpenNewTab, "Abrir link em nova guia");
            }
            else
            {
                //model.SetLabel(CefMenuCommand.Redo, "Refazer");
                //model.SetLabel(CefMenuCommand.Undo, "Desfazer");

                model.Remove(CefMenuCommand.Redo);
                model.Remove(CefMenuCommand.Undo);

                model.SetLabel(CefMenuCommand.Forward, "Avançar");
                model.SetLabel(CefMenuCommand.Back, "Voltar");
                model.SetLabel(CefMenuCommand.Copy, "Copiar");
                //model.SetLabel(CefMenuCommand.AddToDictionary, "Adicionar ao dicionário");
                model.SetLabel(CefMenuCommand.Cut, "Cortar");
                model.SetLabel(CefMenuCommand.Delete, "Apagar");
                model.SetLabel(CefMenuCommand.Paste, "Colar");
                model.SetLabel(CefMenuCommand.SelectAll, "Selecionar tudo");
                model.SetLabel(CefMenuCommand.Print, "Imprimir");
                model.SetLabel(CefMenuCommand.Reload, "Recarregar");
                model.SetLabel(CefMenuCommand.ReloadNoCache, "Voltar");
                model.SetLabel(CefMenuCommand.ViewSource, "Exibir código fonte da página");
                model.AddSeparator(); //indexado 
                model.AddItem((CefMenuCommand)ShowDevTools, "Inspecionar");
            }
        }

        bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            if ((int)commandId == ShowDevTools)
            {
                browser.ShowDevTools();
            }

            if ((int)commandId == OpenNewTab)
            {
                BrowserInterface browserInterface = currentBrowserTab.ParentForm as BrowserInterface;
                browserInterface.InvokeOnUiThreadIfRequired(()=> browserInterface.AddTab(statusLabelLink));
            }
            if ((int)commandId == SearchGoogle)
            {
                BrowserInterface browserInterface = currentBrowserTab.ParentForm as BrowserInterface;
                browserInterface.InvokeOnUiThreadIfRequired(() => browserInterface.AddTab($"https://www.google.com/search?q={selectionTextQuery}"));
            }

            return false;
        }

        void IContextMenuHandler.OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {

        }

        bool IContextMenuHandler.RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }
    }
}