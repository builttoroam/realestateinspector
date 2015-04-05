using System.Collections.Generic;
using System.Text;
using Autofac;
using BuiltToRoam;
using BuiltToRoam.Mobile;
using RealEstateInspector.Core;

namespace RealEstateInspector.Shared.Client
{
    public class ApplicationCore : BaseApplicationCore
    {
        protected override void BuildCoreDependencies(ContainerBuilder builder)
        {
            base.BuildCoreDependencies(builder);

            builder.RegisterType<ConfigurationManager<BuildConfigurationType, AppConfiguration>>()
                .SingleInstance()
                .As<IConfigurationManager<BuildConfigurationType, AppConfiguration>>();


            builder.RegisterType<DataService>().SingleInstance().As<IMobileDataService>().As<IDataService>();
            builder.RegisterType<SyncService>().SingleInstance().As<ISyncService>();

        }

        protected override void CompleteStartup()
        {
            base.CompleteStartup();

            ConfigurationInitializer.Initialize();
        }
    }
}
