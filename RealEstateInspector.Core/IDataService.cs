using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace RealEstateInspector.Core
{
    public interface IDataService
    {
        IMobileServiceClient MobileService { get; }

        Task Initialize(string aadAccessToken);
    }
}