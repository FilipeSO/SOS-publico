// Copyright © 2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Internals;

namespace SOS.Handlers
{
    /// <summary>
    /// WebBrowser extensions - These methods make performing common tasks
    /// easier.
    /// </summary>
    public static class WebBrowserExtensions
    {
        /// <summary>
        /// Execute Javascript code in the context of this WebBrowser. This extension method uses the LoadingStateChanged event.
        /// As the method name implies, the script will be executed asynchronously, and the method therefore returns before the
        /// script has actually been executed.
        /// </summary>
        /// <param name="webBrowser">The ChromiumWebBrowser instance this method extends</param>
        /// <param name="script">The Javascript code that should be executed.</param>
        /// <param name="oneTime">The script will only be executed on first page load, subsiquent page loads will be ignored</param>
        /// <remarks>Best effort is made to make sure the script is executed, there are likely a few edge cases where the script
        /// won't be executed, if you suspect your script isn't being executed, then try executing in the LoadingStateChanged
        /// event handler to confirm that it does indeed get executed.</remarks>
        public static void ExecuteScriptAsyncWhenPageLoaded(this IWebBrowser webBrowser, string script, bool oneTime = true)
        {
            var useLoadingStateChangedEventHandler = webBrowser.IsBrowserInitialized == false || oneTime == false;

            //Browser has been initialized, we check if there is a valid document and we're not loading
            if (webBrowser.IsBrowserInitialized)
            {
                //CefBrowser wrapper
                var browser = webBrowser.GetBrowser();
                if (browser.HasDocument && browser.IsLoading == false)
                {
                    webBrowser.ExecuteScriptAsync(script);
                }
                else
                {
                    useLoadingStateChangedEventHandler = true;
                }
            }

            //If the browser hasn't been initialized we can just wire up the LoadingStateChanged event
            //If the script has already been executed and oneTime is false will be hooked up next page load.
            if (useLoadingStateChangedEventHandler)
            {
                EventHandler<LoadingStateChangedEventArgs> handler = null;

                handler = (sender, args) =>
                {
                    //Wait for while page to finish loading not just the first frame
                    if (!args.IsLoading)
                    {
                        if (oneTime)
                        {
                            webBrowser.LoadingStateChanged -= handler;
                        }

                        webBrowser.ExecuteScriptAsync(script);
                    }
                };

                webBrowser.LoadingStateChanged += handler;
            }
        }

    }
}