using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FluentSkiaSharpControls.Models.Base
{
    public class MutableObject : INotifyPropertyChanged
    {
        #region Auto-implemented
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
