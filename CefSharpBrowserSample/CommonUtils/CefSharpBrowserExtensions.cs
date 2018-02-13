﻿using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CefSharpBrowserSample.CommonUtils
{ /// <summary>
  /// Implemetation of Doenload Handler
  /// </summary>
    public class DownloadHandler : IDownloadHandler
    {
        public void OnBeforeDownload(IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            string downloadPath;
            downloadPath = downloadItem.SuggestedFileName;
        }

        public void OnDownloadUpdated(IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            // DO NOTHING
        }
    }

    /// <summary>
    /// Implementation of Context menu for CefSharp Browser
    /// </summary>
    public class LifeSpanHandler : ILifeSpanHandler
    {
        public bool DoClose(IWebBrowser browserControl, IBrowser browser)
        {
            return false;
        }

        public void OnAfterCreated(IWebBrowser browserControl, IBrowser browser)
        {
            // DO NOTHING
        }

        public void OnBeforeClose(IWebBrowser browserControl, IBrowser browser)
        {
            // DO NOTHING
        }

        public bool OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            try
            {
                System.Diagnostics.Process.Start(targetUrl);
            }
            catch (Exception ex)
            {

            }
            newBrowser = null;
            return true;
        }
    }

    /// <summary>
    /// Dependency Property for Binding content
    /// </summary>
    public class CefSharpBrowserContent
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
                "Html",
                typeof(string),
                typeof(CefSharpBrowserContent),
                new FrameworkPropertyMetadata(OnHtmlChanged));

        /// <summary>
        /// Property To Get Dependency Property
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        [AttachedPropertyBrowsableForType(typeof(CefSharp.Wpf.ChromiumWebBrowser))]
        public static string GetHtml(CefSharp.Wpf.ChromiumWebBrowser d)
        {
            return (string)d.GetValue(HtmlProperty);

        }

        /// <summary>
        /// Property To set Dependency Property
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        public static void SetHtml(CefSharp.Wpf.ChromiumWebBrowser d, string value)
        {
            d.SetValue(HtmlProperty, value);
        }

        /// <summary>
        /// Method is called when frameworkMetaDataChanges
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="e"></param>
        static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var webBrowser = dependencyObject as CefSharp.Wpf.ChromiumWebBrowser;
                if (webBrowser != null && e.NewValue != null)
                    if (string.IsNullOrEmpty(e.NewValue.ToString()))
                    {
                        webBrowser.LoadHtml(string.Empty, "http://example/");
                    }
                    else
                    {
                        if (webBrowser.IsBrowserInitialized)
                            webBrowser.LoadHtml(e.NewValue as string, "http://example/");
                    }
            }
            catch (Exception ex)
            {

            }
        }

    }

    /// <summary>
    /// Extended functionality for CefSharp Browser
    /// </summary>
    public class CefSharpBrowserExtension
    {
        bool isControlKeyPressed = false;
        double maxZoomLevel = 8;
        double minZoomLevel = -8;
        ChromiumWebBrowser browser = new ChromiumWebBrowser();
        public CefSharpBrowserExtension(ChromiumWebBrowser newbrowser)
        {
            browser = newbrowser;
            browser.DownloadHandler = new DownloadHandler();
            browser.LifeSpanHandler = new LifeSpanHandler();
            browser.ContextMenu = new System.Windows.Controls.ContextMenu();
            browser.ZoomLevel = 0;
            browser.ZoomLevelIncrement = 0.5;
            browser.ContextMenu.Items.Add(new MenuItem() { Header = "Select All", Command = browser.SelectAllCommand });
            browser.ContextMenu.Items.Add(new MenuItem() { Header = "Copy", Command = browser.CopyCommand });
            browser.ContextMenu.Items.Add(new MenuItem() { Header = "View Source", Command = browser.ViewSourceCommand });
            browser.IsBrowserInitializedChanged += BrowserHtmlEditor_IsBrowserInitializedChanged;
        }

        private void BrowserHtmlEditor_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!browser.IsBrowserInitialized)
                throw new Exception("Browser not initialized");
        }

    }
}
