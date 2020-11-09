namespace FluentSkiaSharpControls.ViewModels
{
    public abstract class BasePageViewModel : BaseViewModel
    {
        protected BasePageViewModel() { }

        public virtual void OnPageAppearing() { }

        public virtual void OnPageDisappearing() { }
    }
}