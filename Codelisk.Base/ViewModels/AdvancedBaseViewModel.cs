using Prism.AppModel;
using Prism.Magician;
using AsyncAwaitBestPractices;
using Prism.Navigation;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows.Input;
using Codelisk.Base.Constants;

namespace Codelisk.Base.ViewModels
{
    public abstract class AdvancedBaseViewModel : BaseBaseVm,
                                      IInitialize,
                                      IInitializeAsync,
                                      INavigatedAware,
                                      IPageLifecycleAware,
                                      IDestructible,
                                      IConfirmNavigationAsync
    {
        public virtual void SetUpReactive()
        {

        }
        public virtual void OnNavigatedFrom(INavigationParameters parameters) => this.Deactivate();
        public virtual void Initialize(INavigationParameters parameters)
        {
            if (parameters != null)
            {
                if (parameters.TryGetValue(BaseNavigationParameterKeys.PageTitle, out string title))
                {
                    this.Title = title;
                }
            }
            this.SetUpReactive();
        }
        public virtual Task InitializeAsync(INavigationParameters parameters) => Task.CompletedTask;
        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.Back || parameters.ContainsKey(BaseNavigationParameterKeys.SetUpReactiveAgain))
            {
                this.SetUpReactive();
            }
        }
        public virtual void OnAppearing() { }
        public virtual void OnDisappearing() { }
        public virtual void Destroy() => this.DestroyWith?.Dispose();
        public virtual Task<bool> CanNavigateAsync(INavigationParameters parameters) => Task.FromResult(true);



    }
}
