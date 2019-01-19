using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteCollaboration.Util
{
    static class PageSelector
    {
        public static Dictionary<string, Uri> PageDictionary = new Dictionary<string, Uri>()
        {
            { "Startup", new Uri("View/Pages/StartupPage.xaml", UriKind.Relative)},
            { "Collaboration", new Uri("View/Pages/CollaborationPage.xaml", UriKind.Relative)},
        };
    }
}
