using RemoteCollaboration.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace RemoteCollaboration.ViewModel
{
    public class StartupViewModel : ViewModelBase
    {
        #region Binding Properties
        public ICommand StartCommand { get; set; }
        #endregion

        public StartupViewModel(NavigationService navigation) : base(navigation)
        {
            StartCommand = new DelegateCommand(Start);
        }

        /// <summary>
        /// 開始コマンド
        /// </summary>
        /// <param name="paramater"></param>
        public void Start(object paramater)
        {
            Navigate(new Uri("View/Pages/CollaborationPage.xaml", UriKind.Relative), new CollaborationViewModel(NavigationService));
        }
    }
}
