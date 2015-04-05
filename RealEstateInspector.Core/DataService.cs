using System.Diagnostics;
using System.Threading.Tasks;
using BuiltToRoam;
using BuiltToRoam.Mobile;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Newtonsoft.Json.Linq;
using RealEstateInspector.Core.ViewModels;
using RealEstateInspector.Shared.Entities;

namespace RealEstateInspector.Core
{
    public class DataService: IMobileDataService
    {
        public DataService(IConfigurationManager<BuildConfigurationType, AppConfiguration> configManager)
        {
            if (configManager == null) return;
            var config = configManager.Current;
            MobileService = new MobileServiceClient(
            config.MobileServiceRootUri,
            config.MobileServiceApiKey,
            new MobileServiceHttpHandler()
            );
        }


        public IMobileServiceClient MobileService { get; }

        public async Task Initialize()
        {
            var data = new MobileServiceSQLiteStore("inspections.db");
            data.DefineTable<PropertyType>();
            data.DefineTable<RealEstateProperty>();
            data.DefineTable<Inspection>();

            await MobileService.SyncContext.InitializeAsync(data, new CustomMobileServiceSyncHandler());
        }

        public async Task LoginAsync(string aadAccessToken)
        {
            var jobj = new JObject();
            jobj["access_token"] = aadAccessToken;
            var access = await MobileService.LoginAsync(MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory, jobj);
            Debug.WriteLine(access != null);            
        }
    }
}