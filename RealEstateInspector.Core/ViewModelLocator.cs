using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using BuiltToRoam;
using BuiltToRoam.Mobile;
using BuiltToRoam.Navigation;
using BuiltToRoam.ViewModels;
using Microsoft.Practices.ServiceLocation;
using RealEstateInspector.Core.ViewModels;

namespace RealEstateInspector.Core
{
    public class ViewModelLocator //:IViewModelLocator
    {
        public ISettingsService SettingsService { get; set; }
        public IDataService DataService { get; set; }
        public ISyncService SyncService { get; set; }

        public INavigateService NavigateService { get; set; }

        public ViewModelLocator()
        {
            if (!ServiceLocator.IsLocationProviderSet) return;
            SettingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
            DataService = ServiceLocator.Current.GetInstance<IDataService>();
            SyncService = ServiceLocator.Current.GetInstance<ISyncService>();
            NavigateService = ServiceLocator.Current.GetInstance<INavigateService>();
        }

        //public ViewModelLocator(IDataService dataService, ISyncService syncService)
        //{
        //    DataService = dataService;
        //    SyncService = syncService;
        //    //DataService=new DataService();
        //    //SyncService=new SyncService(DataService);
        //}


        public MainViewModel Main
        {
            get { return CreateViewModel<MainViewModel>(); }
        }

        public SecondViewModel Second
        {

            get { return CreateViewModel<SecondViewModel>(); }
        }


        private readonly Dictionary<Type, object> viewModels = new Dictionary<Type, object>();

        private T CreateViewModel<T>() where T : new()
        {
            var type = typeof(T);
            object existing;
            if (!viewModels.TryGetValue(type, out existing))
            {
                existing = new T();
                viewModels[type] = existing;
            }

            var baseVM = existing as IDataViewModel;
            if (baseVM != null)
            {
                baseVM.SettingsService = SettingsService;
                baseVM.DataService = DataService;
                baseVM.NavigateService = NavigateService;
            }
            var baseSyncVM = existing as ISyncViewModel;
            if (baseSyncVM != null)
            {
                baseSyncVM.SyncService = SyncService;
            }

            return (T)existing;
        }
    }

}
