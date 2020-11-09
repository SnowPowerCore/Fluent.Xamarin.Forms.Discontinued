using System;

namespace FluentSkiaSharpControls.ViewModels
{
    /// <summary>
    /// Use this within base view/page to wire typed viewmodel.
    /// Property <see cref="ViewModelType"/> has only setter and should be consumed from xaml
    /// in order to set desired viewmodel.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    public interface IViewBase<TViewModel> where TViewModel : BaseViewModel
    {
        TViewModel TypedViewModel { get; }

        Type ViewModelType { set; }
    }
}