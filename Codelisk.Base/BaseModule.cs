using Prism.Ioc;
using Prism.Modularity;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;

namespace Codelisk.Base
{
    public class BaseModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IDeviceDisplay, DeviceDisplayImplementation>();
            containerRegistry.RegisterSingleton<IDeviceInfo, DeviceInfoImplementation>();
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
            containerRegistry.RegisterSingleton<IFileSystem, FileSystemImplementation>();
        }
    }
}
