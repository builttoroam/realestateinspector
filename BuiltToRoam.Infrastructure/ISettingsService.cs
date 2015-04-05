using System.Threading.Tasks;

namespace BuiltToRoam
{
    public interface ISettingsService
    {
        Task<string> Load(string key);

        Task Save(string key, string value);

    }
}