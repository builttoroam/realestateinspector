using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace RealEstateInspector.Core
{
    public interface ILogWriterService
    {

        IMobileServiceSyncTable<LogEntry> LogTable { get; } 
        
        Task Initialize();
    }
}