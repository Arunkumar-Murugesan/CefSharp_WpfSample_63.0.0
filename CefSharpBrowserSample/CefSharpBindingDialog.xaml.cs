using CefSharpBrowserSample.CommonUtils;
using CefSharpBrowserSample.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CefSharpBrowserSample
{
    /// <summary>
    /// Interaction logic for CefSharpBindingDialog.xaml
    /// </summary>
    public partial class CefSharpBindingDialog : Window
    {
        SupportVM supportVM = new SupportVM();
        public CefSharpBindingDialog()
        {
            InitializeComponent();
            cefBrowserRTE.IsBrowserInitializedChanged += CefBrowserRTE_IsBrowserInitializedChanged;
            new CefSharpBrowserExtension(cefBrowserRTE);
        }

        private void CefBrowserRTE_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(!cefBrowserRTE.IsBrowserInitialized)
            {
                throw new Exception("Browser is not yet initialized");
            }
        }
    }
}
