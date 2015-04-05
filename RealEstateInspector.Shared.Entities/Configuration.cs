#if DEBUG
//#define DEBUGLOCAL
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BuiltToRoam;
using Microsoft.Practices.ServiceLocation;
using RealEstateInspector.Shared.Entities;

namespace RealEstateInspector
{
    public class AppConfiguration : BaseConfiguration
    {

        public AppConfiguration(IDictionary<Expression<Func<string>>, string> initializers) : base(initializers)
        {

        }

        public string ADTenant => Value();

        public string ADAuthority => SharedConstants.ADAuthorityRoot + ADTenant;

        public string ADNativeClientApplicationClientId => Value();

        public string ADRedirectUri => Value();


        public string MobileServiceRootUri => Value();
        public string MobileServiceAppIdUri => Value();

        public string MobileServiceApiKey => Value();




    }

    public enum BuildConfigurationType
    {
        LocalDevelopment,
        Development,
        Test,
        Production
    }

    public static class ConfigurationInitializer
    {
        public static void Initialize()
        {
            var cm =
                ServiceLocator.Current.GetInstance<IConfigurationManager<BuildConfigurationType, AppConfiguration>>();
            AppConfiguration x = null;
            cm.Populate(new Dictionary<BuildConfigurationType, AppConfiguration>
            {
                {
                    BuildConfigurationType.LocalDevelopment, new AppConfiguration
                        (
                        new Dictionary<Expression<Func<string>>, string>
                        {
                            {() => x.ADTenant, "realestateinspector.onmicrosoft.com"},
                            {() => x.ADNativeClientApplicationClientId, "a5a10ee9-f871-4bde-997f-3f1c323fefa5"},
                            {() => x.ADRedirectUri, "http://builttoroam.com"},
                            {() => x.MobileServiceRootUri, "http://localhost:51539/"},
                            {() => x.MobileServiceAppIdUri, "https://realestateinspector.azure-mobile.net/login/aad"},
                            {() => x.MobileServiceApiKey, "wpxaIplpeXtknXQhqXiVlZAPYQEBcg12"}
                        }
                        )
                },
                {
                    BuildConfigurationType.Development, new AppConfiguration(
                        new Dictionary<Expression<Func<string>>, string>
                        {
                            {() => x.ADTenant, "realestateinspector.onmicrosoft.com"},
                            {() => x.ADNativeClientApplicationClientId, "a5a10ee9-f871-4bde-997f-3f1c323fefa5"},
                            {() => x.ADRedirectUri, "http://builttoroam.com"},
                            {() => x.MobileServiceRootUri, "https://realestateinspector.azure-mobile.net/"},
                            {() => x.MobileServiceAppIdUri, "https://realestateinspector.azure-mobile.net/login/aad"},
                            {() => x.MobileServiceApiKey, "wpxaIplpeXtknXQhqXiVlZAPYQEBcg12"}
                        }
                        )
                },
                {
                    BuildConfigurationType.Test, new AppConfiguration(
                        new Dictionary<Expression<Func<string>>, string>
                        {
                            {() => x.ADTenant, "realestateinspector.onmicrosoft.com"},
                            {() => x.ADNativeClientApplicationClientId, "a5a10ee9-f871-4bde-997f-3f1c323fefa5"},
                            {() => x.ADRedirectUri, "http://builttoroam.com"},
                            {() => x.MobileServiceRootUri, "https://realestateinspector.azure-mobile.net/"},
                            {() => x.MobileServiceAppIdUri, "https://realestateinspector.azure-mobile.net/login/aad"},
                            {() => x.MobileServiceApiKey, "wpxaIplpeXtknXQhqXiVlZAPYQEBcg12"}
                        }
                        )
                },
                {
                    BuildConfigurationType.Production, new AppConfiguration(
                        new Dictionary<Expression<Func<string>>, string>
                        {
                            {() => x.ADTenant, "realestateinspector.onmicrosoft.com"},
                            {() => x.ADNativeClientApplicationClientId, "a5a10ee9-f871-4bde-997f-3f1c323fefa5"},
                            {() => x.ADRedirectUri, "http://builttoroam.com"},
                            {() => x.MobileServiceRootUri, "https://realestateinspector.azure-mobile.net/"},
                            {() => x.MobileServiceAppIdUri, "https://realestateinspector.azure-mobile.net/login/aad"},
                            {() => x.MobileServiceApiKey, "wpxaIplpeXtknXQhqXiVlZAPYQEBcg12"}
                        }
                        )
                }
            });

            cm.SelectConfiguration(BuildConfigurationType.Test);
#pragma warning disable 162 // This is to allow for easy override of configuration values to debug issues
            if (false)
            {
                Debug.WriteLine("-----------------WARNING - Default Configuration Values Overridden ------------------");
                cm.SelectConfiguration(BuildConfigurationType.Production);
            }
#pragma warning restore 162

#if DEBUG
#if DEBUGLOCAL
                        cm.SelectConfiguration( BuildConfigurationType.LocalDevelopment);
#else
            cm.SelectConfiguration(BuildConfigurationType.Development);
#endif
#elif TEST
                        cm.SelectConfiguration( BuildConfigurationType.LocalDevelopment);
#else
                        cm.SelectConfiguration( BuildConfigurationType.Production);
#endif

        }
    }
}
