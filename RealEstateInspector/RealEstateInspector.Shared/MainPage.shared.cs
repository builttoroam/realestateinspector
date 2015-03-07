using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Media.Transcoding;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.Storage.Blob;
using RealEstateInspector.Background;
using RealEstateInspector.Core;
using RealEstateInspector.Core.ViewModels;
using RealEstateInspector.Shared.Client;
using Xamarin.Media;
using Microsoft.WindowsAzure.Storage.Auth;
using System.Diagnostics;

namespace RealEstateInspector
{
    public partial class MainPage
    {


        public MainViewModel CurrentViewModel
        {
            get { return DataContext as MainViewModel; }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            CurrentViewModel.Initialize();

            
        }

        private async void AuthenticateClick(object sender, RoutedEventArgs e)
        {

            var token = await AuthenticationHelper.Authenticate();
            await CurrentViewModel.DataService.Initialize(token);
            await CurrentViewModel.LoadPropertyData();
        }

        private async void AddPropertyClick(object sender, RoutedEventArgs e)
        {
            await CurrentViewModel.AddProperty();
        }

        private async void GenerateReportClick(object sender, RoutedEventArgs e)
        {
            CurrentViewModel.GenerateReport();
        }

        private void GeneratePropertyTypeClick(object sender, RoutedEventArgs e)
        {
            CurrentViewModel.GeneratePropertyType();
        }


        private async void CaptureClick(object sender, RoutedEventArgs e)
        {
            var picker = new MediaPicker();
            var sas = string.Empty;
            using (var media = await picker.PickPhotoAsync())
            using (var strm = media.GetStream())
            {
                sas = await CurrentViewModel.RetrieveSharedAccessSignature();
                var container = new CloudBlobContainer(new Uri(sas));
                var blobFromContainer = container.GetBlockBlobReference("testimage" + Path.GetExtension(media.Path));
                await blobFromContainer.UploadFromStreamAsync(strm.AsInputStream());
            }
        }

        private void NavigateClick(object sender, RoutedEventArgs e)
        {
            CurrentViewModel.GoToSecondPage();
        }
    }

    public class WindowsPlatformNavigationService : CoreNavigateService<Page>
    {
        protected override void NavigateToView(Type viewType)
        {
            (Window.Current.Content as Frame).Navigate(viewType);
        }
    }


    public class BackgroundTaskManager
    {
        /// <summary>
        /// Register a background task with the specified taskEntryPoint, name, trigger,
        /// and condition (optional).
        /// </summary>
        /// <param name="taskEntryPoint">Task entry point for the background task.</param>
        /// <param name="name">A name for the background task.</param>
        /// <param name="trigger">The trigger for the background task.</param>
        /// <param name="condition">An optional conditional event that must be true for the task to fire.</param>
        public static async Task<BackgroundTaskRegistration> RegisterBackgroundTask(String taskEntryPoint, String name, IBackgroundTrigger trigger, IBackgroundCondition condition)
        {
            BackgroundExecutionManager.RemoveAccess();
            var hasAccess = await BackgroundExecutionManager.RequestAccessAsync();
            if (hasAccess == BackgroundAccessStatus.Denied) return null;

            var builder = new BackgroundTaskBuilder();
            builder.Name = name;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(trigger);
            BackgroundTaskRegistration task = builder.Register();
            Debug.WriteLine(task);

            return task;
        }

        /// <summary>
        /// Unregister background tasks with specified name.
        /// </summary>
        /// <param name="name">Name of the background task to unregister.</param>
        /// <param name="cancelRunningTask">Flag that cancels or let task finish job</param>
        public static void UnregisterBackgroundTasks(string name, bool cancelRunningTask)
        {
            //
            // Loop through all background tasks and unregister any with SampleBackgroundTaskName or
            // SampleBackgroundTaskWithConditionName.
            //
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == name)
                {
                    cur.Value.Unregister(cancelRunningTask);
                }
            }
        }
    }
}
