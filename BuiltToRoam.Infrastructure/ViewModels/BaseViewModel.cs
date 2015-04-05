using BuiltToRoam;
using BuiltToRoam.Navigation;

namespace BuiltToRoam.ViewModels
{
    public class BaseViewModel : BaseStateAndTransitions, IDataViewModel    {
        public ISettingsService SettingsService { get; set; }
        public IDataService DataService { get; set; }
        public INavigateService NavigateService { get; set; }

        private readonly UIContext context = new UIContext();

        public UIContext UIContext
        {
            get { return context; }
        }


       
    }
}