using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Data;

namespace Ichongli.Rosi
{
    public partial class ImageControl : UserControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty DataContextMonitorProperty =
            DependencyProperty.RegisterAttached("DataContextMonitorProperty",
            typeof(object), typeof(ImageControl), new
                PropertyMetadata(null, new PropertyChangedCallback(OnDataContextMonitorChanged)));

        private static void OnDataContextMonitorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ImageControl).DataContextChanged(e.OldValue, e.NewValue);
        }

        private void DataContextChanged(object p1, object p2)
        {
          
        }

        #endregion

        public ImageControl()
        {
            InitializeComponent();
            this.Loaded += ImageControl_Loaded;
        }

        void ImageControl_Loaded(object sender, RoutedEventArgs e)
        {
            base.SetBinding(DataContextMonitorProperty, new Binding());
        }
    }
}
