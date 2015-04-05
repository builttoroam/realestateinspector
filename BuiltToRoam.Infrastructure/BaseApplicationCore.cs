using System;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Microsoft.Practices.ServiceLocation;

namespace BuiltToRoam
{
    public class BaseApplicationCore
    {
        public void Startup(Action<ContainerBuilder> platformDependencyBuilder)
        {
            var builder = new ContainerBuilder();


            BuildCoreDependencies(builder);

            platformDependencyBuilder(builder);
            // Perform registrations and build the container.
            var container = builder.Build();

            // Set the service locator to an AutofacServiceLocator.
            var csl = new AutofacServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => csl);

            CompleteStartup();
        }

        protected virtual void CompleteStartup()
        {
        }

        protected virtual void BuildCoreDependencies(ContainerBuilder builder)
        {
            
        }

    }
}