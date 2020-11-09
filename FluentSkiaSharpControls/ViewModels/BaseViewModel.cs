using System.ComponentModel;

namespace FluentSkiaSharpControls.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        #region Auto-implemented
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        protected BaseViewModel() { }
    }
}