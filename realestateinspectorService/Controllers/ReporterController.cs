using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;

namespace realestateinspectorService.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    [AuthorizeInspector]
    public class ReporterController : ApiController
    {
        public ApiServices Services { get; set; }

        // GET api/Reporter
        public string Get(string id)
        {
            var host = new ReportHost();
            host.DoWork(async (cancel) =>
            {
                try
                {
                    var hub = Services.GetRealtime<LongRunningFeedbackHub>();

                    var max = 5;
                    for (var i = 0; i < max; i++)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5), cancel);

                        hub.Clients.
                            Client(id)
                            .Progress(string.Format("{0}% complete",100*i/5));
                    }
                }
                catch (Exception ex)
                {
                    // Don't bubble the exception - do something sensible here!
                    Debug.WriteLine(ex.Message);
                }
            });
            Services.Log.Info("Hello from custom controller!");
            return "Hello";
        }
    }


}
