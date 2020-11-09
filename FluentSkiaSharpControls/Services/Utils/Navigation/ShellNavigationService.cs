using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FluentSkiaSharpControls.Services.Utils.Navigation
{
    public class ShellNavigationService : INavigationService
    {
        public Task SwitchMainPageAsync<TPage>(TPage page) where TPage : Page
        {
            if (!(page is Shell)) return Task.CompletedTask;
            Shell.Current.FlyoutIsPresented = false;
            return CloseModalAsync()
                .ContinueWith(t =>
                    Device.InvokeOnMainThreadAsync(() => Application.Current.MainPage = page as Shell),
                        TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public void DetermineAndSetMainPage(string mainRouteName) =>
            Application.Current.MainPage = (Shell)Routing.GetOrCreateContent(mainRouteName);

        public bool CheckCurrentPageType<TType>() =>
            Shell.Current.Navigation.NavigationStack.Count > 1 &&
            Shell.Current.Navigation.NavigationStack.LastOrDefault().GetType().Equals(typeof(TType));

        public Task OpenModalAsync(Page modal, bool animated = true) =>
            Shell.Current.Navigation.PushModalAsync(modal, animated);

        public Task CloseModalAsync(bool animated = true)
        {
            if (Shell.Current.Navigation.ModalStack.Count > 0)
                return Shell.Current.Navigation.PopModalAsync(animated);

            return Task.CompletedTask;
        }

        public Task OpenPopupAsync(string routeWithParams, bool animated = true) =>
           PopupNavigation.Instance.PushAsync(GetElementFromRouting<PopupPage>(routeWithParams), animated);

        public Task ClosePopupAsync(bool animated = true)
        {
            if (PopupNavigation.Instance.PopupStack.Count > 0)
                return PopupNavigation.Instance.PopAsync(animated);
            else return Task.CompletedTask;
        }

        private T GetElementFromRouting<T>(string routeWithParams) where T : Element
        {
            var routeAndParams = routeWithParams.Split('?');
            if (routeAndParams.Length > 1)
            {
                var routeName = routeAndParams.FirstOrDefault();
                var query = routeAndParams.LastOrDefault();
                var queryData = ParseQueryString(query);
                var page = (T)Routing.GetOrCreateContent(routeName);
                var attrs = (QueryPropertyAttribute[])Attribute.GetCustomAttributes(
                    page.GetType(),
                    typeof(QueryPropertyAttribute));
                if (attrs != null && attrs.Length > 0)
                {
                    foreach (var attr in attrs)
                    {
                        queryData.TryGetValue(attr.QueryId, out var value);
                        var prop = page
                            .GetType()
                            .GetProperty(attr.Name, BindingFlags.Public | BindingFlags.Instance);
                        if (null != prop && prop.CanWrite)
                            prop.SetValue(page, value, null);
                    }
                }
                return page;
            }
            else
            {
                return (T)Routing.GetOrCreateContent(routeWithParams);
            }
        }

        private Dictionary<string, string> ParseQueryString(string query)
        {
            if (query.StartsWith("?", StringComparison.Ordinal))
                query = query.Substring(1);
            var lookupDict = new Dictionary<string, string>();
            if (query == null)
                return lookupDict;
            foreach (var part in query.Split('&'))
            {
                var p = part.Split('=');
                if (p.Length != 2)
                    continue;
                lookupDict[p[0]] = p[1];
            }

            return lookupDict;
        }

        public Task NavigateToPageAsync(string routeWithParams, bool animated = true)
        {
            Shell.Current.FlyoutIsPresented = false;
            return Shell.Current.GoToAsync(routeWithParams, animated);
        }

        public Task NavigateBackAsync(bool animated = true)
        {
            if (Shell.Current.Navigation.NavigationStack.Count > 1)
                return Shell.Current.Navigation.PopAsync(animated);
            return Task.CompletedTask;
        }

        public Task NavigateToRootAsync(bool animated = true)
        {
            Shell.Current.FlyoutIsPresented = false;
            return Shell.Current.Navigation.PopToRootAsync(animated);
        }

        public Task SwitchItemAsync(int index) =>
            Device.InvokeOnMainThreadAsync(() =>
                Shell.Current.CurrentItem = Shell.Current.Items.ElementAtOrDefault(index));
    }
}