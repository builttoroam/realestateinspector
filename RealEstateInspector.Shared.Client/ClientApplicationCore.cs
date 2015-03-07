using System;
#if SILVERLIGHT
using System.IO.IsolatedStorage;
#endif
using System.Windows;
using Autofac;
using MetroLog;
using Microsoft.Practices.ServiceLocation;
using RealEstateInspector.Core;
using RealEstateInspector.Core.ViewModels;
using RealEstateInspector.XForms;

//using FileAccess = System.IO.FileAccess;
#if DESKTOP
using RealEstateInspector.Desktop.Pages;
#endif

namespace RealEstateInspector.Shared.Client
{
    public class ClientApplicationCore
    {
        public static ClientApplicationCore Default { get; set; }

        static ClientApplicationCore()
        {
            Default = new ClientApplicationCore();
        }

        public ClientApplicationCore()
        {
            CoreApplication = new ApplicationCore();
        }

        public ApplicationCore CoreApplication { get; set; }

        public void ApplicationStartup()
        {
            CoreApplication.Startup(builder =>
            {
                builder.RegisterType<PlatformSettingsService>().SingleInstance().As<ISettingsService>();

                builder.RegisterType<SignalRFactory>().As<ISignalR>();
#if NETFX_CORE
                builder.RegisterType<UniversalUIContext>().As<IUIContext>();
                builder.RegisterType<WindowsPlatformNavigationService>().SingleInstance().As<INavigateService>();

#elif DESKTOP
                builder.RegisterType<WPFNavigationService>().SingleInstance().As<INavigateService>();
#elif SILVERLIGHT || DROID || __IOS__
                builder.RegisterType<XFormsNavigationService>().SingleInstance().As<INavigateService>();
#endif
                builder.RegisterType<LogWriterService>().As<ILogWriterService>();
                builder.RegisterType<LogService>().As<ILogService>();

            });

            LogHelper.Log("Startup complete");
            try
            {
#if NETFX_CORE
                var navService = ServiceLocator.Current.GetInstance<INavigateService>() as WindowsPlatformNavigationService;
#elif DESKTOP
            var navService = ServiceLocator.Current.GetInstance<INavigateService>() as WPFNavigationService;
#endif
#if NETFX_CORE || DESKTOP
                navService.Register<MainViewModel, MainPage>();
                navService.Register<SecondViewModel, SecondPage>();
#endif
            }
            catch (Exception ex)
            {
                ex.Log();
            }
        }
    }
}




