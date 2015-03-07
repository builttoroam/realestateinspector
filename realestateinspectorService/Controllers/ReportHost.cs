using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace realestateinspectorService.Controllers
{
    public class ReportHost : IRegisteredObject
    {
        private readonly ManualResetEvent reportLock = new ManualResetEvent(false);
        private readonly CancellationTokenSource cancellation=new CancellationTokenSource();

        public ReportHost()
        {
            HostingEnvironment.RegisterObject(this);
        }

        private int stopped;
        public void Stop(bool immediate)
        {
            
            if (immediate)
            {
                cancellation.Cancel();
            }
            reportLock.WaitOne();

            // Make sure this is only ever called once to unregister the object
            if (Interlocked.CompareExchange(ref stopped, 0, 1) == 1) return;
            HostingEnvironment.UnregisterObject(this); 
        }

        public void DoWork(Func<CancellationToken,Task> work)
        {
            Task.Run(async () =>
            {
                await work(cancellation.Token);
                reportLock.Set();
                Stop(false);
            });
        }
    }
}