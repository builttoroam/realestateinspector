#if DEBUG
//#define DEBUGLOCAL
#endif

using System.Collections.Generic;
using System.Diagnostics;
using RealEstateInspector.Shared.Entities;

namespace RealEstateInspector
{
    public class Configuration
    {
        static Configuration()
        {
#pragma warning disable 162 // This is to allow for easy override of configuration values to debug issues
            if (false)
            {
                Debug.WriteLine("-----------------WARNING - Default Configuration Values Overridden ------------------");
                Current = Configurations[ConfigurationType.Production];
            }
#pragma warning restore 162

#if DEBUG
#if DEBUGLOCAL
            Current = Configurations[ConfigurationType.LocalDevelopment];
#else
            Current = Configurations[ConfigurationType.Development];
#endif
#elif TEST
            Current = Configurations[ConfigurationType.LocalDevelopment];
#else
            Current = Configurations[ConfigurationType.Production];
#endif
        }

        public static Configuration Current { get; set; }

        public string ADTenant { get; set; }

        public string ADAuthority
        {
            get { return SharedConstants.ADAuthorityRoot + ADTenant; }
        }

        public string ADNativeClientApplicationClientId { get; set; }

        public string ADRedirectUri { get; set; }


        public string MobileServiceRootUri { get; set; }
        public string MobileServiceAppIdUri { get; set; }

        public string MobileServiceApiKey { get; set; }

        public enum ConfigurationType
        {
            LocalDevelopment,
            Development,
            Test,
            Production
        }

        public static IDictionary<ConfigurationType, Configuration> Configurations
        {
            get { return configurations; }
        }

        private static readonly IDictionary<ConfigurationType, Configuration> configurations
            = new Dictionary<ConfigurationType, Configuration>
            {
                {
                    ConfigurationType.LocalDevelopment, new Configuration
                    {
                        ADTenant = "realestateinspector.onmicrosoft.com",
                        ADNativeClientApplicationClientId = "a5a10ee9-f871-4bde-997f-3f1c323fefa5",
                        ADRedirectUri = "http://builttoroam.com",
                        MobileServiceRootUri = "http://localhost:51539/",
                        MobileServiceAppIdUri = "https://realestateinspector.azure-mobile.net/login/aad",
                        MobileServiceApiKey="wpxaIplpeXtknXQhqXiVlZAPYQEBcg12"
                    }
                },
                {
                    ConfigurationType.Development, new Configuration
                    {
                        ADTenant = "realestateinspector.onmicrosoft.com",
                        ADNativeClientApplicationClientId = "a5a10ee9-f871-4bde-997f-3f1c323fefa5",
                        ADRedirectUri = "http://builttoroam.com",
                        MobileServiceRootUri = "https://realestateinspector.azure-mobile.net/",
                        MobileServiceAppIdUri = "https://realestateinspector.azure-mobile.net/login/aad",
                        MobileServiceApiKey="wpxaIplpeXtknXQhqXiVlZAPYQEBcg12"
                    }
                },
                {
                    ConfigurationType.Test, new Configuration
                    {
                        ADTenant = "realestateinspector.onmicrosoft.com",
                        ADNativeClientApplicationClientId = "a5a10ee9-f871-4bde-997f-3f1c323fefa5",
                        ADRedirectUri = "http://builttoroam.com",
                        MobileServiceRootUri = "https://realestateinspector.azure-mobile.net/",
                        MobileServiceAppIdUri = "https://realestateinspector.azure-mobile.net/login/aad",
                        MobileServiceApiKey="wpxaIplpeXtknXQhqXiVlZAPYQEBcg12"
                    }
                },
                {
                    ConfigurationType.Production, new Configuration
                    {
                        ADTenant = "realestateinspector.onmicrosoft.com",
                        ADNativeClientApplicationClientId = "a5a10ee9-f871-4bde-997f-3f1c323fefa5",
                        ADRedirectUri = "http://builttoroam.com",
                        MobileServiceRootUri = "https://realestateinspector.azure-mobile.net/",
                        MobileServiceAppIdUri = "https://realestateinspector.azure-mobile.net/login/aad",
                        MobileServiceApiKey="wpxaIplpeXtknXQhqXiVlZAPYQEBcg12"
                    }
                }
            };
    }
}
