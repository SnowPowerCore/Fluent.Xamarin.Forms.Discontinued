using System;
using Xamarin.Forms;
using FluentSkiaSharpControls.ViewModels;

namespace FluentSkiaSharpControls.Views.Pages
{
    public partial class BasePageView : ContentPage, IViewBase<BasePageViewModel>
    {
        public BasePageView() =>
            InitializeComponent();

        public Type ViewModelType
        {
            set
            {
                BindingContext = null != value
                    ? App.Services.GetService(value)
                    : throw new Exception($"Couldn't locate viewmodel of type {value}.");

                if (!(BindingContext is BasePageViewModel))
                    throw new Exception($"Binding context should inherit from {nameof(BasePageViewModel)}.");
            }
        }

        public BasePageViewModel TypedViewModel =>
           (BasePageViewModel)BindingContext;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is null)
                throw new Exception("ViewModelType should be set before page appears.");

            TypedViewModel?.OnPageAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            TypedViewModel?.OnPageDisappearing();
        }
    }
}