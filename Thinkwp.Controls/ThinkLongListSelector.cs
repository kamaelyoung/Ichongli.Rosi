using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Thinkwp.Controls
{
    /// <summary>
    /// Add DataRequest event ，when LonglistSelector scroll to end
    /// raise DataRequeset event to notifity load next page data
    /// </summary>
    public class ThinkLongListSelector : LongListSelector
    {
        private const int Offset = 2;

        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(ThinkLongListSelector),
                new PropertyMetadata(default(bool)));

        public ThinkLongListSelector()
        {
            ItemRealized += OnItemRealized;
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

        private void OnItemRealized(object sender, ItemRealizationEventArgs itemRealizationEventArgs)
        {
            if (!IsLoading && ItemsSource != null && ItemsSource.Count >= Offset)
            {
                if (itemRealizationEventArgs.ItemKind == LongListSelectorItemKind.Item)
                {
                    object offsetItem = ItemsSource[ItemsSource.Count - Offset];
                    if ((itemRealizationEventArgs.Container.Content == offsetItem))
                    {
                        OnDataRequest();
                    }
                }
            }
        }
    }
}
