#if SERVICE
using Microsoft.AspNet.SignalR;
using Microsoft.WindowsAzure.Mobile.Service;
#endif

namespace realestateinspectorService
{
    public class LongRunningFeedbackHub
#if SERVICE
        : Hub
    {
        public ApiServices Services { get; set; }
#else
    {
        
#endif
    }
}