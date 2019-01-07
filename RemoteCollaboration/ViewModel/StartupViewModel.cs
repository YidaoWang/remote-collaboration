using RemoteCollaboration.Model;
using RemoteCollaboration.Util;
using System;
using System.Windows.Input;
using System.Windows.Navigation;

namespace RemoteCollaboration.ViewModel
{
    public class StartupViewModel : ViewModelBase
    {
        #region Binding Properties
        public ICommand StartCommand { get; set; }
        public Experiment Experiment { get; set; }
        #endregion

        public StartupViewModel(NavigationService navigation, Experiment exp = null) : base(navigation)
        {
            StartCommand = new DelegateCommand(Start);
            Experiment = exp ?? new Experiment();
        }

        /// <summary>
        /// 開始コマンド
        /// </summary>
        /// <param name="paramater"></param>
        public void Start(object paramater)
        {
            Navigate(new Uri("View/Pages/CollaborationPage.xaml", UriKind.Relative), new CollaborationViewModel(NavigationService, Experiment));
        }
    }
}
