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
using Parse;

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

            ParseClient.Initialize("guC2YcdOyhm16rpuB1tDo4U6elEgZGH17AC2t7sX", "XezmH3Srdmbdh4eIXCiYUjl9N9d6Va1RISWcEvrC");
        }

    }
}