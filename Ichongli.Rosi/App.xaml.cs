﻿using System;
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
        ///提供对电话应用程序的根框架的轻松访问。
        /// </summary>
        /// <returns>电话应用程序的根框架。</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        private Cookie _cookie;
        public Cookie Cookie
        {
            get
            {
                return this._cookie;
            }
            set
            {
                this._cookie = value;
            }
        }

        public static App Current
        {
            get
            {
                return (App)Application.Current;
            }
        }

        /// <summary>
        /// Application 对象的构造函数。
        /// </summary>
        public App()
        {
       
            // 标准 XAML 初始化
            InitializeComponent();
        }

    }
}