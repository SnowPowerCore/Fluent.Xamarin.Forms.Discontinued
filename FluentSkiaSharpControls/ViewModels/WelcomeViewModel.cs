using System.Windows.Input;
using Xamarin.Forms;

namespace FluentSkiaSharpControls.ViewModels
{
    public class WelcomeViewModel : BasePageViewModel
    {
        private bool _magicProperty = false;

        private ICommand _updateStateCommand;

        public bool MagicProperty
        {
            get => _magicProperty;
            set
            {
                _magicProperty = value;
                OnPropertyChanged();
            }
        }

        public ICommand UpdateStateCommand => _updateStateCommand ??
            (_updateStateCommand = new Command(() => MagicProperty = !MagicProperty));
    }
}
