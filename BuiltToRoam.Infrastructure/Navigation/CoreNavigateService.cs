using System;
using System.Collections.Generic;
using BuiltToRoam.ViewModels;

namespace BuiltToRoam.Navigation
{
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
}