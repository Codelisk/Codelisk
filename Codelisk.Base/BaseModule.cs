using Codelisk.Base.ViewModels;
using Codelisk.Base.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace Codelisk.Base
{
    public class BaseModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ViewA, ViewAViewModel>();
        }
    }
}
