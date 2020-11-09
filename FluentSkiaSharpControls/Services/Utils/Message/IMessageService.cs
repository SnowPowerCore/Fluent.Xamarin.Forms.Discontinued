using System.Threading.Tasks;

namespace FluentSkiaSharpControls.Services.Utils.Message
{
    public interface IMessageService : IToastService
    {
        Task DisplayInfoAsync(string message);

        Task DisplayErrorDescOnlyAsync(string errorDesc);

        Task<bool> DisplayConfirmationAsync(string dialogName, string dialogDesc,
            string confirmLabel, string denyLabel);

        Task<string> DisplayInputAsync(string title = "", string message = "",
            string inputText = "", string inputPlaceholder = "");
    }
}