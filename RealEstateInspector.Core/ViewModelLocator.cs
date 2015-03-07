using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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
                baseVM.SyncService = SyncService;
                baseVM.NavigateService = NavigateService;
            }

            return (T)existing;
        }
    }

    public interface INavigateService
    {
        void Navigate<TViewModel>() where TViewModel : IDataViewModel;
    }

    public interface INativeNavigateService<TView> : INavigateService
        where TView : class,new()
    {
        void Register<TViewModel, TViewType>() where TViewType : TView;
    }

    public abstract class CoreNavigateService<TView> : INativeNavigateService<TView> where TView : class, new()
    {
        private readonly IDictionary<Type, Type> viewDictionary = new Dictionary<Type, Type>();

        protected Type ViewType<TViewModel>()
        {
            Type viewType = null;
            viewDictionary.TryGetValue(typeof(TViewModel), out viewType);
            return viewType;
        }

        public void Register<TViewModel, TViewType>() where TViewType : TView
        {
            viewDictionary[typeof(TViewModel)] = typeof(TViewType);
        }

        public void Navigate<TViewModel>() where TViewModel : IDataViewModel
        {
            var navType = ViewType<TViewModel>();
            NavigateToView(navType);
        }

        protected abstract void NavigateToView(Type viewType);
    }

    //public interface IViewModelLocator
    //{
    //}

    //public static class LocatorFactor
    //{
    //    public static IViewModelLocator Locator
    //    {
    //        get
    //        {
    //            var locator = ServiceLocator.Current.GetInstance<IViewModelLocator>();
    //            return locator;
    //        }
    //    }
    //}
}
