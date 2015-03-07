using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Windows.ApplicationModel.Background;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Practices.ServiceLocation;
using RealEstateInspector.Core;
using RealEstateInspector.Shared.Entities;

namespace RealEstateInspector.Background
{

    public sealed class SyncTask : IBackgroundTask
    {
        private BackgroundTaskCancellationReason cancelReason = BackgroundTaskCancellationReason.Abort;
        //private volatile bool cancelRequested = false;
        private BackgroundTaskDeferral deferral = null;

        private ISyncService SyncService { get; set; }
        //
        // The Run method is the entry point of a background task.
        //

        private static Mutex foregroundMutex = new Mutex(false,"BackgroundSync");
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            try
            {
                if (!foregroundMutex.WaitOne(0)) return;
                foregroundMutex.ReleaseMutex();

                Debug.WriteLine("Background " + taskInstance.Task.Name + " Starting...");

                //
                // Get the deferral object from the task instance, and take a reference to the taskInstance;
                //
                deferral = taskInstance.GetDeferral();

                //
                // Associate a cancellation handler with the background task.
                //
                taskInstance.Canceled += OnCanceled;

                //
                // Query BackgroundWorkCost
                // Guidance: If BackgroundWorkCost is high, then perform only the minimum amount
                // of work in the background task and return immediately.
                var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;

                var authContext = new AuthenticationContext(Configuration.Current.ADAuthority);
                if (authContext.TokenCache.ReadItems().Count() > 0)
                {
                    authContext = new AuthenticationContext(authContext.TokenCache.ReadItems().First().Authority);
                }

                var authResult =
                    await
                        authContext.AcquireTokenSilentAsync(Configuration.Current.MobileServiceAppIdUri,
                        Configuration.Current.ADNativeClientApplicationClientId);
                if (authResult != null && !string.IsNullOrWhiteSpace(authResult.AccessToken))
                {
                    var dataService = ServiceLocator.Current.GetInstance<IDataService>();
                    SyncService = ServiceLocator.Current.GetInstance<ISyncService>();

                    await dataService.Initialize(authResult.AccessToken);


                    if (cost == BackgroundWorkCostValue.High)
                    {
                        await SyncService.ForceUpload();
                    }
                    else
                    {
                        await SyncService.Synchronise(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                if (deferral != null)
                {
                    deferral.Complete();
                }
            }
        }

        //
        // Handles background task cancellation.
        //
        private async void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            var deferral = sender.GetDeferral();

            try
            {
                if (SyncService != null)
                {
                    await SyncService.Cancel();
                }
                cancelReason = reason;
                Debug.WriteLine("Background " + sender.Task.Name + " Cancel Requested...");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            deferral.Complete();
        }
    }
}
