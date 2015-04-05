using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BuiltToRoam.Navigation;

namespace RealEstateInspector
{
    public class WindowsPlatformNavigationService : CoreNavigateService<Page>
    {
        protected override void NavigateToView(Type viewType)
        {
            (Window.Current.Content as Frame).Navigate(viewType);
        }
    }
}