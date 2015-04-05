using BuiltToRoam.ViewModels;

namespace BuiltToRoam.Navigation
{
    public interface INavigateService
    {
        void Navigate<TViewModel>() where TViewModel : IDataViewModel;
    }
}