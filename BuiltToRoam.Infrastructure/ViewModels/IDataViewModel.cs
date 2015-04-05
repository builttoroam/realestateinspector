using BuiltToRoam;
using BuiltToRoam.Navigation;

namespace BuiltToRoam.ViewModels
{
    public interface IDataViewModel
    {
        ISettingsService SettingsService { get; set; }
        IDataService DataService { get; set; }

        INavigateService NavigateService { get; set; }
    }
}