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
using Microsoft.Phone.Controls;

namespace ThinkWP.Controls
{
    public class TiltControl : ContentControl
    {
        public TiltControl()
        {
            if (!TiltEffect.TiltableItems.Contains(this.GetType()))
            {
                TiltEffect.TiltableItems.Add(this.GetType());
            }

            TiltEffect.SetIsTiltEnabled(this, true);
        }
    }
}
