using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Caliburn.Micro.Coding4Fun
{
    public class Coding4FunWindowManager : IWindowManager
    {
        public void ShowDialog(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            ShowDialog(rootModel, context, (Brush)Application.Current.Resources["PhoneSemitransparentBrush"], settings);
        }

        public void ShowPopup(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            ShowDialog(rootModel, context, null, settings);
        }

        private static void ShowDialog(object rootModel, object context, Brush overlay, IEnumerable<KeyValuePair<string, object>> settings)
        {
            var dialog = new Coding4FunDialog
            {
                Context = context,
                RootModel = rootModel,
                Overlay = overlay
            };

            if (settings != null)
            {
                var type = dialog.GetType();
                foreach (var setting in settings)
                {
                    var propertyInfo = type.GetProperty(setting.Key);
                    if (propertyInfo != null)
                    {
                        propertyInfo.SetValue(dialog, setting.Value, null);
                    }
                }
            }

            var activate = rootModel as IActivate;
            if (activate != null)
            {
                activate.Activate();
            }

            var deactivate = rootModel as IDeactivate;
            if (deactivate != null)
            {
                dialog.Completed += (sender, args) => deactivate.Deactivate(true);
            }

            dialog.Show();
        }
    }
}