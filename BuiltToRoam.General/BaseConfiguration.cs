using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace BuiltToRoam
{
    public abstract class BaseConfiguration
    {
        protected IDictionary<string,string> Data { get; } = new Dictionary<string,string>();

        protected string Value([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null) return null;
            return Data.SafeDictionaryValue<string, string, string>(propertyName);
        }

        protected BaseConfiguration(IDictionary<Expression<Func<string>>, string> initializers=null)
        {
            if (initializers == null) return;
            initializers.DoForEach(initializer =>
                Data[(initializer.Key.Body as MemberExpression).Member.Name] = initializer.Value);

        }


    }

    public class ConfigurationManager<TConfigurationKey, TConfiguration> : IConfigurationManager<TConfigurationKey,TConfiguration>
        where TConfigurationKey : struct
        where TConfiguration :BaseConfiguration
    {
        private IDictionary<TConfigurationKey, TConfiguration> Configurations { get; } = new Dictionary<TConfigurationKey, TConfiguration>();


        public void Populate(IDictionary<TConfigurationKey, TConfiguration> configurations)
        {
            if (configurations == null) return;
            configurations.DoForEach(d=>Configurations[d.Key]=d.Value);
        }

        public void SelectConfiguration(TConfigurationKey key)
        {
            Current = Configurations.SafeDictionaryValue<TConfigurationKey, TConfiguration, TConfiguration>(key);
        }

        public TConfiguration Current { get; private set; }

    }

    public interface IConfigurationManager<TConfigurationKey, TConfiguration>
        where TConfigurationKey : struct
        where TConfiguration : BaseConfiguration
    {
        void Populate(IDictionary<TConfigurationKey, TConfiguration> configurations);

        void SelectConfiguration(TConfigurationKey key);

        TConfiguration Current { get; }
    }
}
