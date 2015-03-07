using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuiltToRoam;
using BuiltToRoam.Synchronization;
using RealEstateInspector.Core.ViewModels;
using RealEstateInspector.Shared.Entities;

namespace RealEstateInspector.Core
{
    public class SyncService: ISyncService
    {
        [Flags]
        private enum SyncStages
        {
            None = 0,
            UploadChanges = 1,
            PullPropertyTypes = 2,
            PullProperties = 4,
            PullInspections = 8,
            All = UploadChanges | PullPropertyTypes | PullProperties | PullInspections
        }

        public event EventHandler<DualParameterEventArgs<double,string>> Progress; 

        public IDataService DataService { get; set; }

        private ISynchronizationContext<SyncStages> SynchronizationManager { get; set; }

        public SyncService(IDataService dataService)
        {
            DataService = dataService;
            SynchronizationManager = new SynchronizationContext<SyncStages>();
            SynchronizationManager.DefineSynchronizationStep(SyncStages.UploadChanges, UploadPendingLocalChanges);
            SynchronizationManager.DefineSynchronizationStep(SyncStages.PullPropertyTypes, DownloadChangesToPropertyTypes);
            SynchronizationManager.DefineSynchronizationStep(SyncStages.PullProperties, DownloadChangesToRealEstateProperties);
            SynchronizationManager.DefineSynchronizationStep(SyncStages.PullInspections, DownloadChangesToInspections);
            SynchronizationManager.SynchronizationChanged += SynchronizationManager_SynchronizationProgressChanged;
        }

        public async Task Synchronise(bool waitForCompletion)
        {
            await SynchronizationManager.Synchronize(SyncStages.All, waitForSynchronizationToComplete: waitForCompletion);
        }

        public async Task ForceUpload()
        {
            await SynchronizationManager.Synchronize(SyncStages.UploadChanges, true, true);
        }

        public async Task Cancel()
        {
            await SynchronizationManager.Cancel(true);
        }

        private void SynchronizationManager_SynchronizationProgressChanged(object sender, SynchronizationEventArgs<SyncStages> e)
        {
            var message = e.ToString();
            if (Progress != null)
            {
                Progress(this,new object[]{ e.PercentageComplete, message});
            }
        }

        private async Task<bool> UploadPendingLocalChanges(ISynchronizationStage<SyncStages> stage)
        {
            await DataService.MobileService.SyncContext.PushAsync(stage.CancellationToken);
            return true;
        }
        private async Task<bool> DownloadChangesToPropertyTypes(ISynchronizationStage<SyncStages> stage)
        {
            await DataService.MobileService.PullLatestAsync<PropertyType>(stage.CancellationToken);
            return true;
        }
        private async Task<bool> DownloadChangesToRealEstateProperties(ISynchronizationStage<SyncStages> stage)
        {
            await DataService.MobileService.PullLatestAsync<RealEstateProperty>(stage.CancellationToken);
            return true;
        }
        private async Task<bool> DownloadChangesToInspections(ISynchronizationStage<SyncStages> stage)
        {
            await DataService.MobileService.PullLatestAsync<Inspection>(stage.CancellationToken);
            return true;
        }
    }
}
