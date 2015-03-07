using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Newtonsoft.Json.Linq;
using RealEstateInspector.Core.ViewModels;
using RealEstateInspector.Shared.Entities;

namespace RealEstateInspector.Core
{
    public class DataService: IDataService
    {
        private readonly MobileServiceClient mobileService = new MobileServiceClient(
            Configuration.Current.MobileServiceRootUri,
            Configuration.Current.MobileServiceApiKey,
            new MobileServiceHttpHandler()
            );

        public IMobileServiceClient MobileService
        {
            get { return mobileService; }
        }

        public async Task Initialize(string aadAccessToken)
        {
            var jobj = new JObject();
            jobj["access_token"] = aadAccessToken;
            var access = await MobileService.LoginAsync(MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory, jobj);
            Debug.WriteLine(access != null);
            var data = new MobileServiceSQLiteStore("inspections.db");
            data.DefineTable<PropertyType>();
            data.DefineTable<RealEstateProperty>();
            data.DefineTable<Inspection>();

            await MobileService.SyncContext.InitializeAsync(data, new CustomMobileServiceSyncHandler());

        }
    }
}