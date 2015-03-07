using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;
using PCLStorage;
using RealEstateInspector.Shared.Client;
using RealEstateInspector.Shared.Entities;
using realestateinspectorService;

namespace RealEstateInspector.Core.ViewModels
{
    public class MainViewModel : BaseViewModel
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

            var props = await DataService.MobileService.GetSyncTable<RealEstateProperty>().ToListAsync();
            foreach (var prop in props)
            {
                Properties.Add(prop);
            }
        }

        public async Task AddProperty()
        {
            var table = DataService.MobileService.GetSyncTable<RealEstateProperty>();
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
                var hub = await signalr.Connect<LongRunningFeedbackHub>(DataService.MobileService);
                hub.Register<string>("Progress",
                    async msg =>
                        await UIContext.RunAsync(() =>
                        {
                            Progress = msg;
                        })
                        );


                var result = await DataService.MobileService.InvokeApiAsync<string>("Reporter", HttpMethod.Get, new Dictionary<string, string> { { "id", hub.ConnectionId } });

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
            var sas = await DataService.MobileService.InvokeApiAsync<string>("sharedaccesssignature", HttpMethod.Get,
                new Dictionary<string, string> { { "id", "test" } });
            return sas;
        }

        public void GoToSecondPage()
        {
            NavigateService.Navigate<SecondViewModel>();
        }

        public async void GeneratePropertyType()
        {
            var sas = await DataService.MobileService.GetSyncTable<PropertyType>().ToListAsync();
            foreach (var pt in sas)
            {
                if (pt.Name == "Apartment") return;
            }
            await
                DataService.MobileService.GetSyncTable<PropertyType>()
                    .InsertAsync(new PropertyType {Name = "Apartment"});
            await SyncService.ForceUpload();
        }
    }

    public interface IDataViewModel
    {
        ISettingsService SettingsService { get; set; }
        IDataService DataService { get; set; }
        ISyncService SyncService { get; set; }

        INavigateService NavigateService { get; set; }
    }

    public class NotifyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));

        }
    }

    public class BaseViewModel : NotifyBase, IDataViewModel
    {
        public ISettingsService SettingsService { get; set; }
        public IDataService DataService { get; set; }
        public ISyncService SyncService { get; set; }
        public INavigateService NavigateService { get; set; }

        private readonly UIContext context = new UIContext();

        public UIContext UIContext
        {
            get { return context; }
        }

       
    }

    public interface IUIContext
    {
        Task RunOnUIThreadAsync(Func<Task> action);
    }


    public class UIContext
    {
        private IUIContext runContext;
        private IUIContext RunContext
        {
            get
            {
                if (runContext == null)
                {
                    runContext = ServiceLocator.Current.GetInstance<IUIContext>();

                }
                return runContext;
            }
        }

        public async Task RunAsync(Action action)
        {
#pragma warning disable 1998 // Required to force to Task overloaded method
            await RunAsync(async () => action());
#pragma warning restore 1998
        }

        public async Task RunAsync(Func<Task> action)
        {
            var context = RunContext;
            await context.RunOnUIThreadAsync(action);
        }
    }


    public class CustomMobileServiceSyncHandler : MobileServiceSyncHandler
    {
        public async override Task<JObject> ExecuteTableOperationAsync(IMobileServiceTableOperation operation)
        {
            try
            {
                return await base.ExecuteTableOperationAsync(operation);
            }
            catch (MobileServiceConflictException cex)
            {
                Debug.WriteLine(cex.Message);
                throw;
            }
            catch (MobileServicePreconditionFailedException pex)
            {
                Debug.WriteLine(pex.Message);
                throw;
            }
            catch (MobileServicePushFailedException pfex)
            {
                Debug.WriteLine(pfex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public override Task OnPushCompleteAsync(MobileServicePushCompletionResult result)
        {
            foreach (var error in result.Errors)
            {
                if (error.Status == HttpStatusCode.Conflict)
                {
                    error.CancelAndUpdateItemAsync(error.Result);
                    error.Handled = true;
                }
            }
            return base.OnPushCompleteAsync(result);
        }
    }

    public class MobileServiceHttpHandler : DelegatingHandler
    {
        public static string RefreshToken { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(RefreshToken))
            {
                request.Headers.Add(SharedConstants.RefreshTokenHeaderKey, RefreshToken);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
    public static class MobileServiceClientExtensions
    {
        public async static Task PullLatestAsync<TTable>(this IMobileServiceClient client, CancellationToken token) where TTable : BaseEntityData
        {
            // Get the most recent
            var mostRecent = await client.GetLatestAsync<TTable>();

            // Convert the most recent into a query (assuming there is one)
            if (mostRecent != null)
            {
                var maxTimestamp = mostRecent.UpdatedAt.AddMilliseconds(1);
                var q = client.GetSyncTable<TTable>()
                    .CreateQuery()
                    .Where(x => (x.Id != mostRecent.Id || x.UpdatedAt > maxTimestamp));
                // Do a (filtered) pull from the remote tabl
                await client.GetSyncTable<TTable>().PullAsync(typeof(TTable).Name, q, token);
            }
            else
            {
                await client.GetSyncTable<TTable>().PullAsync(typeof(TTable).Name, client.GetSyncTable<TTable>().CreateQuery(), token);
            }
        }

        public async static Task<TTable> GetLatestAsync<TTable>(this IMobileServiceClient client) where TTable : BaseEntityData
        {
            return (await client.GetSyncTable<TTable>()
                                .OrderByDescending(x => x.UpdatedAt)
                                .Take(1)
                                .ToListAsync()).SingleOrDefault();
        }
    }
}
