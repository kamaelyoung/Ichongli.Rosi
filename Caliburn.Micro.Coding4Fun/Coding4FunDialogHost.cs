using System;
using System.Windows.Controls;

namespace Caliburn.Micro.Coding4Fun
{
    public class Coding4FunDialogHost : ContentControl
    {
        public event EventHandler Closed;

        protected virtual void OnClosed()
        {
            var handler = Closed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void Close()
        {
            OnClosed();
        }
    }
}