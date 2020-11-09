using System.Threading.Tasks;
using Xamarin.Forms;

namespace FluentSkiaSharpControls.Services.Utils.Navigation
{
    public interface INavigationService
    {
        Task SwitchMainPageAsync<TPage>(TPage page) where TPage : Page;

        void DetermineAndSetMainPage(string mainRouteName);

        bool CheckCurrentPageType<TType>();

        Task SwitchItemAsync(int index);

        Task OpenPopupAsync(string routeWithParams, bool animated = true);

        Task ClosePopupAsync(bool animated = true);

        Task NavigateToPageAsync(string routeWithParams, bool animated = true);

        Task NavigateBackAsync(bool animated = true);

        Task NavigateToRootAsync(bool animated = true);

        Task OpenModalAsync(Page modal, bool animated = true);

        Task CloseModalAsync(bool animated = true);
    }
}