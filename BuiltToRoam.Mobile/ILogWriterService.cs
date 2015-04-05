using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace BuiltToRoam.Mobile
{
    public interface ILogWriterService
    {

        IMobileServiceSyncTable<LogEntry> LogTable { get; } 
        
        Task Initialize();
    }
}