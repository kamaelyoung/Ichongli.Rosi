using Caliburn.Micro;

namespace Ichongli.Rosi.ViewModels
{
    public class DialogViewModel : Screen
    {
        private string _text;
        private string _title;
        private DialogResult _result = DialogResult.Cancel;

        public string Text
        {
            get { return _text; }
            set
            {
                if (value == _text) return;
                _text = value;
                NotifyOfPropertyChange(() => Text);
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if (value == _title) return;
                _title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        public DialogResult Result
        {
            get { return _result; }
            set
            {
                if (value == _result) return;
                _result = value;
                NotifyOfPropertyChange(() => Result);
            }
        }

        public void Ok()
        {
            // put your stuff here
            Result = DialogResult.Ok;
            TryClose();
        }
    }

    public enum DialogResult
    {
        Ok, Cancel
    }
}
