using System;
using System.Threading.Tasks;
using BuiltToRoam;
using BuiltToRoam.Mobile;

namespace BuiltToRoam.Mobile
{
    public interface ISyncService
    {
        IMobileDataService DataService { get; set; }

        event EventHandler<DualParameterEventArgs<double, string>> Progress;
        Task Synchronise(bool waitForCompletion); 
        Task ForceUpload();
        Task Cancel();
    }
}