using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace CefSharpBrowserSample.ViewModel
{
    public class SupportVM : INotifyPropertyChanged

        {
        public string _loadHTMLDetails = "";
        public string LoadHTMLDetails
        {
            get { return _loadHTMLDetails; }
            set
            {
                _loadHTMLDetails = value;
                OnPropertyChanged("LoadHTMLDetails");
            }
        }

        #region INotify Property Change Implementation

        /// <summary>
        /// On Property change event
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Property change event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        ///Gets or sets the command for load content
        /// </summary>
        /// 
        private ICommand _commandLoadContent;
        public ICommand CommandLoadContent
        {
            get
            {
                if (this._commandLoadContent == null)
                {
                    this._commandLoadContent = new RelayCommand(this.LoadContent, CanLoadContent);
                }
                return this._commandLoadContent;
            }
        }
        CefSharpBindingDialog browserBindingWindow = null;
        private void LoadContent()
        {
            LoadHTMLDetails = "<html><head></head><body><h1>Hello, World!</h1></body></html";
            browserBindingWindow = new CefSharpBindingDialog();
            browserBindingWindow.DataContext = this;
            browserBindingWindow.Owner = System.Windows.Application.Current != null ? System.Windows.Application.Current.MainWindow : null;
            browserBindingWindow.ShowDialog();
        }

        private bool CanLoadContent()
        {
            return true;
        }
    }
}
