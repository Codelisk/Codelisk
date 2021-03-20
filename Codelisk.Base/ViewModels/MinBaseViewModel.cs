using Codelisk.Base.Constants;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codelisk.Base.ViewModels
{
    public partial class MinBaseViewModel :
        BaseBaseVm,
        IInitialize,
        IDestructible,
        INavigatedAware
    {
        public virtual void Destroy() => this.DestroyWith?.Dispose();
        public virtual void SetUpReactive()
        {

        }
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
        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.Back || parameters.ContainsKey(BaseNavigationParameterKeys.SetUpReactiveAgain))
            {
                this.SetUpReactive();
            }
        }
        public virtual void OnNavigatedFrom(INavigationParameters parameters) => this.Deactivate();
    }
}
