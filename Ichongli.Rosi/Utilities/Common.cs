namespace Ichongli.Rosi.Utilities
{
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    public class Common
    {
        public static CustomMessageBox MsgBoxShow(string caption, string leftContent, string rightContent, string msgText)
        {
            SystemTray.Opacity = (0.99);
            CustomMessageBox customMessageBox = new CustomMessageBox();
            customMessageBox.Message = ((object)msgText).ToString();
            customMessageBox.Caption = caption;
            customMessageBox.LeftButtonContent = (object)leftContent;
            customMessageBox.RightButtonContent = (object)rightContent;
            customMessageBox.Show();
            return customMessageBox;
        }
    }
}
