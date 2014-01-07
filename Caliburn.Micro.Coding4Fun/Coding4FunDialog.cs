using System.ComponentModel;
using System.Windows;
using Coding4Fun.Toolkit.Controls;
using Coding4Fun.Toolkit.Controls.Common;
using Microsoft.Phone.Controls;

namespace Caliburn.Micro.Coding4Fun
{
    public class Coding4FunDialog : PopUp<object, PopUpResult>
    {
        public static readonly DependencyProperty RootModelProperty =
            DependencyProperty.Register("RootModel", typeof(object), typeof(Coding4FunDialog), new PropertyMetadata(null));

        public static readonly DependencyProperty ContextProperty =
            DependencyProperty.Register("Context", typeof(object), typeof(Coding4FunDialog), new PropertyMetadata(null));

        private PhoneApplicationPage _page;

        public object RootModel
        {
            get { return GetValue(RootModelProperty); }
            set { SetValue(RootModelProperty, value); }
        }

        public object Context
        {
            get { return GetValue(ContextProperty); }
            set { SetValue(ContextProperty, value); }
        }

        public Coding4FunDialog()
        {
            DefaultStyleKey = typeof(Coding4FunDialog);
        }

        public bool IgnoreBackKey
        {
            get { return IsBackKeyOverride; }
            set { IsBackKeyOverride = value; }
        }

        protected PhoneApplicationPage Page
        {
            get
            {
                if (_page != null) return _page;

                var frame = ApplicationSpace.RootFrame as PhoneApplicationFrame;
                if (frame != null)
                {
                    var page = frame.Content as PhoneApplicationPage;
                    if (page != null)
                    {
                        return _page = page;
                    }
                }

                return null;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (IgnoreBackKey && Page != null)
            {
                Page.BackKeyPress += PageOnBackKeyPress;
            }

            var host = (Coding4FunDialogHost)GetTemplateChild("ViewContainer");
            var view = ViewLocator.LocateForModel(RootModel, host, Context);
            host.Content = view;
            host.SetValue(View.IsGeneratedProperty, true);

            ViewModelBinder.Bind(RootModel, host, null);
            Action.SetTarget(host, RootModel);

            host.Closed += (sender, args) =>
            {
                if (Page != null)
                {
                    Page.BackKeyPress -= PageOnBackKeyPress;
                }
                OnCompleted(new PopUpEventArgs<object, PopUpResult>
                {
                    Result = PopUpResult.Ok
                });
            };
        }

        private void PageOnBackKeyPress(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}