using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace RemoteCollaboration.Util
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public ViewModelBase(NavigationService navigation)
        {
            NavigationService = navigation;
        }

        protected NavigationService NavigationService;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void Navigate(Uri source, object navigationState)
        {
            NavigationService.Navigate(source, navigationState);
        }
    }
}
