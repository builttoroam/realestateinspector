using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Microsoft.Practices.ServiceLocation;
using Microsoft.WindowsAzure.MobileServices;
using RealEstateInspector.Core;

namespace RealEstateInspector.Shared.Client
{
    public class ApplicationCore
    {
        public void Startup(Action<ContainerBuilder> dependencyBuilder)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<DataService>().SingleInstance().As<IDataService>();
            builder.RegisterType<SyncService>().SingleInstance().As<ISyncService>();

            dependencyBuilder(builder);
            // Perform registrations and build the container.
            var container = builder.Build();

            // Set the service locator to an AutofacServiceLocator.
            var csl = new AutofacServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => csl);
        }
    }

    public interface ISignalR
    {
        Task<ICommunicationHub> Connect<THub>(IMobileServiceClient mobileService);
    }

    public interface ICommunicationHub:IDisposable
    {
        string ConnectionId { get; }

        IDisposable Register<TMessageType>(string eventName, Action<TMessageType> handler);
    }
}
