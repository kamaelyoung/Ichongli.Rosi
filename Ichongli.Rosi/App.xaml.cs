using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ichongli.Rosi.Resources;
using System.Net;

namespace Ichongli.Rosi
{
    public partial class App : Application
    {
        /// <summary>
        /// Application 对象的构造函数。
        /// </summary>
        public App()
        {
            // 标准 XAML 初始化
            InitializeComponent();
            UmengSDK.UmengAnalytics.Init("52e2044956240b5a320c1d26");
            UmengSDK.UmengAnalytics.IsDebug = true;//是否输出调试信息

        }

    }
}