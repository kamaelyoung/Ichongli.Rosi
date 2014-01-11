using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Parse;

namespace Ichongli.Rosi.Views
{
    public partial class PostPage : PhoneApplicationPage
    {
        public PostPage()
        {
            InitializeComponent(); 
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var json = ParsePush.PushJson(e);
            object objectId;
            if (json.TryGetValue("objectId", out objectId))
            {
                MessageBox.Show(objectId as string);
            }
            base.OnNavigatedTo(e);
        }
    }
}