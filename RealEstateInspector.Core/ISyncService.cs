using System;
using System.Threading.Tasks;
using BuiltToRoam;

namespace RealEstateInspector.Core
{
    public interface ISyncService
    {
        event EventHandler<DualParameterEventArgs<double, string>> Progress;
        Task Synchronise(bool waitForCompletion);
        Task ForceUpload();
        Task Cancel();
    }
}