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

namespace Ichongli.Controls
{
    public class ThinkLonglistselector: LongListSelector {
        private const int Offset = 2;

        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(ThinkLonglistselector),
                new PropertyMetadata(default(bool)));

        public ThinkLonglistselector()
        {
            ItemRealized += OnItemRealized;
        }

        public bool IsLoading {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public event EventHandler DataRequest;

        protected virtual void OnDataRequest() {
            EventHandler handler = DataRequest;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void OnItemRealized(object sender, ItemRealizationEventArgs itemRealizationEventArgs) {
            if (!IsLoading && ItemsSource != null && ItemsSource.Count >= Offset) {
                if (itemRealizationEventArgs.ItemKind == LongListSelectorItemKind.Item) {
                    object offsetItem = ItemsSource[ItemsSource.Count - Offset];
                    if ((itemRealizationEventArgs.Container.Content == offsetItem)) {
                        OnDataRequest();
                    }
                }
            }
        }
    }
}
