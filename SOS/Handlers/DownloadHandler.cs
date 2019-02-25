// Copyright © 2013 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using CefSharp;
using CefSharp.WinForms.Internals;
using System;
using System.IO;

namespace SOS.Handlers
{
    public class DownloadHandler : IDownloadHandler
    {
        private BrowserTabUserControl currentBrowserTab = null;

        public DownloadHandler(BrowserTabUserControl form)
        {
            currentBrowserTab = form;
        }
        public event EventHandler<DownloadItem> OnBeforeDownloadFired;

        public event EventHandler<DownloadItem> OnDownloadUpdatedFired;

        public void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            OnBeforeDownloadFired?.Invoke(this, downloadItem);

            if (!callback.IsDisposed)
            {
                using (callback)
                {
                    callback.Continue(downloadItem.SuggestedFileName, showDialog: true);
                }
            }
        }

        public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            if (downloadItem.IsInProgress)
            {
                currentBrowserTab.InvokeOnUiThreadIfRequired(() =>
                {
                    if (!currentBrowserTab.downloadOutputLabel.Visible) currentBrowserTab.downloadOutputLabel.Visible = true;
                    currentBrowserTab.downloadOutputLabel.Enabled = false;
                    //currentBrowserTab.downloadOutputLabel.LinkArea = new System.Windows.Forms.LinkArea(currentBrowserTab.downloadOutputLabel.Text.Length, currentBrowserTab.downloadOutputLabel.Text.Length);
                    currentBrowserTab.downloadOutputLabel.Text = $"Download {downloadItem.SuggestedFileName}: {((float)downloadItem.CurrentSpeed/1000000).ToString("0.00")} MB/s - {((float)downloadItem.ReceivedBytes/1000000).ToString("0.00")} MB de {((float)downloadItem.TotalBytes/1000000).ToString("0.00")} MB, destino: {downloadItem.FullPath}";  
                    
                });
            }
            if (downloadItem.IsComplete)
            {
                currentBrowserTab.InvokeOnUiThreadIfRequired(() =>
                {
                    currentBrowserTab.downloadOutputLabel.Enabled = true;
                    FileInfo downloadItemFile = new FileInfo(downloadItem.FullPath);
                    currentBrowserTab.downloadOutputLabel.Text = $"Download {downloadItemFile.Name} concluído disponível na pasta {downloadItemFile.DirectoryName}";
                    currentBrowserTab.downloadOutputLabel.Links.Add($"Download {downloadItemFile.Name} concluído disponível na pasta ".Length, downloadItemFile.DirectoryName.Length, downloadItemFile.DirectoryName);
                });                
            }
            //OnDownloadUpdatedFired?.Invoke(this, downloadItem);
        }
    }
}
