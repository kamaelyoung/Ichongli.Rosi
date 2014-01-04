using Caliburn.Micro;

namespace Ichongli.Rosi.ViewModels
{
    public class MessageViewModel : Screen
    {
        private string _text;
        private string _title;

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

        public void Ok()
        {
            TryClose();
        }
    }
}
