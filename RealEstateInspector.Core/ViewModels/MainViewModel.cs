using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BuiltToRoam.Communication;
using BuiltToRoam.Mobile.ViewModels;
using BuiltToRoam.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using PCLStorage;
using RealEstateInspector.Shared.Client;
using RealEstateInspector.Shared.Entities;
using realestateinspectorService;

namespace RealEstateInspector.Core.ViewModels
{
    public class RealEstateBaseViewModel : BaseMobileViewModel
    {
        
    }

    public class MainViewModel : RealEstateBaseViewModel
    {
        private readonly ObservableCollection<RealEstateProperty> properties = new ObservableCollection<RealEstateProperty>();

        public ObservableCollection<RealEstateProperty> Properties
        {
            get { return properties; }
        }


        public void Initialize()
        {
            SyncService.Progress += SyncService_Progress;

        }

        async void SyncService_Progress(object sender, BuiltToRoam.DualParameterEventArgs<double, string> e)
        {
            await UIContext.RunAsync(() => Progress = e.Parameter2);
        }

        

        private string progress;

        public string Progress
        {
            get { return progress; }
            set
            {
                if (Progress == value) return;
                progress = value;
                OnPropertyChanged();
            }
        }


        public async Task LoadPropertyData()
        {
            await SyncService.Synchronise(true);

            var props = await SyncService.DataService.MobileService.GetSyncTable<RealEstateProperty>().ToListAsync();
            foreach (var prop in props)
            {
                Properties.Add(prop);
            }
        }

        public async Task AddProperty()
        {
            var table = SyncService.DataService.MobileService.GetSyncTable<RealEstateProperty>();
            var prop = new RealEstateProperty
            {
                Address = "New Random Property"
            };
            await table.InsertAsync(prop);

            await SyncService.ForceUpload();
        }

        public async void GenerateReport()
        {
            string message;
            try
            {
                var signalr = ServiceLocator.Current.GetInstance<ISignalR>();
                var mobileService = SyncService.DataService.MobileService;
                var hub = await signalr.Connect<LongRunningFeedbackHub>(mobileService.ApplicationUri.AbsoluteUri,
                new Dictionary<string, string> { { "x-zumo-application", mobileService.ApplicationKey } });
                hub.Register<string>("Progress",
                    async msg =>
                        await UIContext.RunAsync(() =>
                        {
                            Progress = msg;
                        })
                        );


                var result = await SyncService.DataService.MobileService.InvokeApiAsync<string>("Reporter", HttpMethod.Get, new Dictionary<string, string> { { "id", hub.ConnectionId } });

                message = result;
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                message = ex.Message;
            }
            Debug.WriteLine(message);
        }

        private string fileText;

        public string FileText
        {
            get { return fileText; }
            set
            {
                if (FileText == value) return;
                fileText = value;
                OnPropertyChanged();
            }
        }


        public async Task WriteFile()
        {
            var file =
                await
                    FileSystem.Current.LocalStorage.CreateFileAsync("test.txt", CreationCollisionOption.ReplaceExisting);
            using (var stream = await file.OpenAsync(FileAccess.ReadAndWrite))
            using (var writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(FileText);
            }
        }
        public async Task ReadFile()
        {
            var file =
                await
                    FileSystem.Current.LocalStorage.GetFileAsync("test.txt");
            using (var stream = await file.OpenAsync(FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                FileText = await reader.ReadToEndAsync();
            }
        }

        public async Task WriteImageFile(string fileName, Stream imageContents)
        {
            var file =
                await
                    FileSystem.Current.LocalStorage.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            using (var stream = await file.OpenAsync(FileAccess.ReadAndWrite))
            {
                await imageContents.CopyToAsync(stream);
            }
        }

        //public async Task<string> SaveAndUploadImage(Stream strm)
        public async Task<string> RetrieveSharedAccessSignature()
        {
            var sas = await SyncService.DataService.MobileService.InvokeApiAsync<string>("sharedaccesssignature", HttpMethod.Get,
                new Dictionary<string, string> { { "id", "test" } });
            return sas;
        }

        public void GoToSecondPage()
        {
            NavigateService.Navigate<SecondViewModel>();
        }

        public async void GeneratePropertyType()
        {
            var sas = await SyncService.DataService.MobileService.GetSyncTable<PropertyType>().ToListAsync();
            foreach (var pt in sas)
            {
                if (pt.Name == "Apartment") return;
            }
            await
                SyncService.DataService.MobileService.GetSyncTable<PropertyType>()
                    .InsertAsync(new PropertyType {Name = "Apartment"});
            await SyncService.ForceUpload();
        }
    }
}
