using BuiltToRoam;
using BuiltToRoam.Navigation;
using BuiltToRoam.ViewModels;

namespace BuiltToRoam.Mobile.ViewModels
{
    public class BaseMobileViewModel : BaseViewModel, ISyncViewModel
    {
        public ISyncService SyncService { get; set; }
       
    }
}