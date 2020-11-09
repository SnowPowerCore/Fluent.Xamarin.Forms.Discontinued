using System.Threading.Tasks;
using Xamarin.Forms;
using FluentSkiaSharpControls.Resources;
using FluentSkiaSharpControls.Services.Interfaces;

namespace FluentSkiaSharpControls.Services.Utils.Message
{
    public class MessageService : IMessageService
    {
        #region Fields
        private readonly IToast _toast;
        private bool _displayingError = false;
        #endregion

        #region Constructor
        public MessageService(IToast toast)
        {
            _toast = toast;
        }
        #endregion

        #region Methods
        public Task<bool> DisplayConfirmationAsync(string dialogName,
            string dialogDesc, string confirmLabel, string denyLabel) =>
                Application.Current.MainPage.DisplayAlert(dialogName, dialogDesc, confirmLabel, denyLabel);

        public Task DisplayErrorDescOnlyAsync(string errorDesc)
        {
            if (_displayingError) return Task.CompletedTask;
            _displayingError = true;
            return Application.Current.MainPage.DisplayAlert("", errorDesc, AppResources.cancel)
                .ContinueWith(t => _displayingError = false);
        }

        public Task DisplayInfoAsync(string message) =>
            Application.Current.MainPage.DisplayAlert("", message, AppResources.cancel);

        public Task<string> DisplayInputAsync(string title = "",
            string message = "", string inputText = "", string inputPlaceholder = "") =>
                Application.Current.MainPage.DisplayPromptAsync(title, message,
                    initialValue: inputText, placeholder: inputPlaceholder);

        public void DisplayToast(string info) =>
            _toast.ShowToast(info);
        #endregion
    }
}