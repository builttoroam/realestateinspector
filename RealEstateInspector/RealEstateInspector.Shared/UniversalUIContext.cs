﻿using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using BuiltToRoam;
using RealEstateInspector.Core.ViewModels;

namespace RealEstateInspector
{
    public class UniversalUIContext : IUIContext
    {
        public async Task RunOnUIThreadAsync(Func<Task> action)
        {
            var awaiter = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await action());
            awaiter.GetResults();
        }

    }
}