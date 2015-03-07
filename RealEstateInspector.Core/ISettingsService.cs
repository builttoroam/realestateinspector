using System.Threading.Tasks;

namespace RealEstateInspector.Core
{
    public interface ISettingsService
    {
        Task<string> Load(string key);

        Task Save(string key, string value);

    }
}