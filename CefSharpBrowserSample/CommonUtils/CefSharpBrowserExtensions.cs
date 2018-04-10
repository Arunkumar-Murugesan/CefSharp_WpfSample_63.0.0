using CefSharp;
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
{
    /// <summary>
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
                //new ErrorLog().LogExceptionMessageToFile("CefSharpBrowserExtensions.cs - LifeSpanHandler()" + ex.StackTrace);
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
                {
                    var url = e.NewValue.ToString();
                    //Browser is initialized, so we'll load the Url
                    if (webBrowser.IsBrowserInitialized)
                    {
                        if (string.IsNullOrEmpty(url))
                        {
                            webBrowser.Load("data:text/html,Invalid Url Specified");
                        }
                        else
                        {
                            webBrowser.LoadHtml(url, "http://example/");
                        }
                    }
                    else
                    {
                        //Browser is not initliazed we'll wait then load
                        DependencyPropertyChangedEventHandler handler = null;
                        handler = (sender, args) =>
                        {
                            webBrowser.IsBrowserInitializedChanged -= handler;

                            //If browser is intialized then it's safe to load
                            if (webBrowser.IsBrowserInitialized)
                            {
                                webBrowser.LoadHtml(url, "http://example/");
                            }
                        };

                        webBrowser.IsBrowserInitializedChanged += handler;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// This method is used ignore script error for web browser control
        /// </summary>
        private static void HideScriptErrors(CefSharp.Wpf.ChromiumWebBrowser wb, bool hide)
        {
            System.Reflection.FieldInfo fieldComWebBrowser = typeof(CefSharp.Wpf.ChromiumWebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldComWebBrowser == null) return;
            object objComWebBrowser = fieldComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null) return;
            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
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
            //browser.ContextMenu.Items.Add(new MenuItem() { Header = "Zoom In", Command = browser.ZoomInCommand });
            //browser.ContextMenu.Items.Add(new MenuItem() { Header = "Zoom Out", Command = browser.ZoomOutCommand });
            //browser.ContextMenu.Items.Add(new MenuItem() { Header = "Reset", Command = browser.ZoomResetCommand });
            browser.MouseWheel += OnMouseWheel;
            browser.PreviewKeyDown += OnKPreviewKeyDown;
            browser.PreviewKeyUp += OnPreviewKeyUp;
        }

        private void OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.RightCtrl || e.Key == Key.LeftCtrl)
                {
                    isControlKeyPressed = false;
                }
            }
            catch (Exception ex)
            {
                //new ErrorLog().LogExceptionMessageToFile("CefSharpBrowserExtensions.cs - OnPreviewKeyUp()" + ex.StackTrace);
            }
        }

        private void OnKPreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.RightCtrl || e.Key == Key.LeftCtrl)
                {
                    isControlKeyPressed = true;
                }
            }
            catch (Exception ex)
            {
                //   new ErrorLog().LogExceptionMessageToFile("CefSharpBrowserExtensions.cs - OnKPreviewKeyDown()" + ex.StackTrace);
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            try
            {
                if (isControlKeyPressed)
                {
                    if (e.Delta > 0 && browser.ZoomLevel < maxZoomLevel)
                    {
                        browser.ZoomInCommand.Execute(null);
                    }
                    else if (e.Delta < 0 && browser.ZoomLevel > minZoomLevel)
                    {
                        browser.ZoomOutCommand.Execute(null);
                    }
                }
            }
            catch (Exception ex)
            {
                //new ErrorLog().LogExceptionMessageToFile("CefSharpBrowserExtensions.cs - OnMouseWheel()" + ex.StackTrace);
            }
        }

    }
}
