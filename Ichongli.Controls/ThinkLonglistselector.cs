using Microsoft.Phone.Controls;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ThinkWP.Controls
{
    public class ThinkLonglistselector : LongListSelector
    {
        private const int Offset = 2;

        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(ThinkLonglistselector),
                new PropertyMetadata(default(bool)));

        public ThinkLonglistselector()
        {
            ItemRealized += ThinkLonglistselector_ItemRealized;
            ItemUnrealized += ThinkLonglistselector_ItemUnrealized;
        }

        void ThinkLonglistselector_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (!IsLoading && ItemsSource != null && ItemsSource.Count >= Offset)
            {
                if (e.ItemKind == LongListSelectorItemKind.Item)
                {
                    object offsetItem = ItemsSource[ItemsSource.Count - Offset];
                    if ((e.Container.Content == offsetItem))
                    {
                        OnDataRequest();
                    }
                }
            }
        }

        void ThinkLonglistselector_ItemUnrealized(object sender, ItemRealizationEventArgs e)
        {
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public event EventHandler DataRequest;

        protected virtual void OnDataRequest()
        {
            EventHandler handler = DataRequest;
            if (handler != null) handler(this, EventArgs.Empty);
        }

    }
}
