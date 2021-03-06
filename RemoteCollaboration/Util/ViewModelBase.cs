﻿using System;
using System.ComponentModel;
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

        protected void Navigate(string pageKey, object navigationState)
        {
            NavigationService.Navigate(PageSelector.PageDictionary[pageKey], navigationState);
        }
    }
}
