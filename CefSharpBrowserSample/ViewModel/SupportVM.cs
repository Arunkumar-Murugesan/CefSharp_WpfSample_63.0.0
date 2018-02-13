using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefSharpBrowserSample.ViewModel
{
    public class SupportVM : ViewModelBase,INotifyPropertyChanged

        {
        /// </summary>
        private string _updateHistory = "<html><head></head><body><hl>Hello World</h1></body></html>";
        public string UpdateHistory
        {
            get { return _updateHistory; }
            set
            {
                this._updateHistory = value;
                this.RaisePropertyChanged("UpdateHistory");
            }
        }
    }
}
