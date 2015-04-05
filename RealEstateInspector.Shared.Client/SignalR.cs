#if !WINDOWS_UAP
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BuiltToRoam.Communication;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.WindowsAzure.MobileServices;
using RealEstateInspector.Core;

namespace RealEstateInspector.Shared.Client
{
    internal class SignalRFactory : ISignalR
    {


        public async Task<ICommunicationHub> Connect<THub>(IMobileServiceClient mobileService)
        {
            return await Connect<THub>(mobileService.ApplicationUri.AbsoluteUri,
                new Dictionary<string, string> { { "x-zumo-application", mobileService.ApplicationKey } });
            //ds.MobileService.ApplicationUri.AbsoluteUri
        }
        public async Task<ICommunicationHub> Connect<THub>(string endpointUri, IDictionary<string, string> headers = null)
        {

            // TODO: This is not good!
            var ds = (new DataService(null));
            var hubConnection = new HubConnection(endpointUri);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    hubConnection.Headers[header.Key] = header.Value;
                }
            }
            //hubConnection.Headers["x-zumo-application"] = ds.MobileService.ApplicationKey;

            IHubProxy proxy = hubConnection.CreateHubProxy(typeof(THub).Name);
            await hubConnection.Start();

            return new CommunicationHub { Connection = hubConnection, Hub = proxy };
        }
    }

    internal class CommunicationHub : ICommunicationHub
    {
        public string ConnectionId { get { return Connection.ConnectionId; } }
        public HubConnection Connection { get; set; }
        public IHubProxy Hub { get; set; }
        public IDisposable Register<TMessageType>(string eventName, Action<TMessageType> handler)
        {
            return Hub.On(eventName, handler);
        }

        public void Dispose()
        {
            if (Connection != null)
            {
                Connection.Dispose();
                Connection = null;
                Hub = null;
            }

        }
    }
}
#endif