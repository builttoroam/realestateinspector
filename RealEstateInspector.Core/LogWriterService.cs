using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using RealEstateInspector.Shared.Entities;

namespace RealEstateInspector.Core
{
    public class LogWriterService : ILogWriterService
    {
        private readonly MobileServiceClient mobileService = new MobileServiceClient(Configuration.Current.MobileServiceRootUri);

        private IMobileServiceClient MobileService
        {
            get { return mobileService; }
        }

        public IMobileServiceSyncTable<LogEntry> LogTable
        {
            get { return MobileService.GetSyncTable<LogEntry>(); }
        }

        public async Task Initialize()
        {
            var data = new MobileServiceSQLiteStore("log.db");
            data.DefineTable<LogEntry>();

            await MobileService.SyncContext.InitializeAsync(data, new MobileServiceSyncHandler());
        }
    }
}