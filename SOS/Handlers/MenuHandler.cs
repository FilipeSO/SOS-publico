// Copyright © 2014 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using CefSharp;
using System;
using SOS.Controls;

namespace SOS.Handlers
{
    internal class MenuHandler : IContextMenuHandler
    {
        private static BrowserTabUserControl _instanceForm = null;

        public MenuHandler(BrowserTabUserControl form)
        {
            _instanceForm = form;
        }
        
        private const int ShowDevTools = 26501;
        private const int CloseDevTools = 26502;
        private const int OpenNewTab = 26503;


        void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            //To disable the menu then call clear
            //model.Clear();

            //Removing existing menu item
            //bool removed = model.Remove(CefMenuCommand.ViewSource); // Remove "View Source" option
            //model.SetLabel(CefMenuCommand.ViewSource, "AAAAA");
            //Add new custom menu items
            //model.AddItem((CefMenuCommand)ShowDevTools, "Show DevTools");
            model.AddItem((CefMenuCommand)OpenNewTab, "Abrir link em nova guia");
            model.AddSeparator(); //conta no indexx
            model.AddItem((CefMenuCommand)CloseDevTools, "Close DevTools");
            string text = _instanceForm.statusLabel.Text;
            model.SetEnabledAt(0, Uri.IsWellFormedUriString(_instanceForm.statusLabel.Text, UriKind.RelativeOrAbsolute) && !string.IsNullOrWhiteSpace(text));
        }

        bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            if ((int)commandId == ShowDevTools)
            {
                browser.ShowDevTools();
            }
            if ((int)commandId == CloseDevTools)
            {
                browser.CloseDevTools();
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