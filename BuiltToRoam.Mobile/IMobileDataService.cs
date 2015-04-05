using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace BuiltToRoam.Mobile
{
    public interface IMobileDataService : IDataService
    {
        IMobileServiceClient MobileService { get; }

        Task LoginAsync(string aadAccessToken);
    }
}