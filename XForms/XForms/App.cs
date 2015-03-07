using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RealEstateInspector.Core;
using RealEstateInspector.Core.ViewModels;
using Xamarin.Forms;
using RealEstateInspector.Shared.Client;


namespace RealEstateInspector.XForms
{
    public class App : Application
    {
        
        public App()
        {

            Resources = new ResourceDictionary();
            var locator = new ViewModelLocator();
            Resources.Add("Locator", locator);

            MainPage =new MainPage(); 
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }

    public class XFormsNavigationService : INativeNavigateService<Page>

    {
        public void Register<TViewModel, TViewType>() where TViewType : Page
        {
        }

        public void Navigate<TViewModel>() where TViewModel : IDataViewModel
        {
        }
    }
}
