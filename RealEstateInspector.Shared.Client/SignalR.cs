using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.WindowsAzure.MobileServices;
using RealEstateInspector.Core;

namespace RealEstateInspector.Shared.Client
{
    internal class SignalRFactory : ISignalR
    {
        public async Task<ICommunicationHub> Connect<THub>(IMobileServiceClient mobileService)
        {
            // TODO: This is not good!
            var ds = (new DataService());
            var hubConnection = new HubConnection(ds.MobileService.ApplicationUri.AbsoluteUri);

            hubConnection.Headers["x-zumo-application"] = ds.MobileService.ApplicationKey;

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
