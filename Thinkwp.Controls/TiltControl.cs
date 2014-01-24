namespace Thinkwp.Controls
{
    using System.Windows.Controls;
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
