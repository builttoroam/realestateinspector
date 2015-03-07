using RealEstateInspector.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using BuiltToRoam;
using Microsoft.Practices.ServiceLocation;
using RealEstateInspector.Core;
using RealEstateInspector.Core.ViewModels;
using System.Globalization;
#if DROID
using Xamarin.Forms;
#elif !NETFX_CORE
using System.Windows.Data;
#else
using Windows.ApplicationModel;
using Windows.UI.Xaml.Data;
#endif

namespace RealEstateInspector.Shared.Client
{
    public abstract class FormDisplayConverter : IValueConverter
    {
        protected IDictionary<PropertyInfo, FormElement> DisplayProperties { get; set; }

        public FormDisplayConverter()
        {
            DisplayProperties = new Dictionary<PropertyInfo, FormElement>();
            AllProperties = new Dictionary<Type, PropertyInfo[]>();
            DefineProperties();
        }

        protected abstract void DefineProperties();

        private IDictionary<Type, PropertyInfo[]> AllProperties { get; set; }

        protected void DefineForm<TProp>(Expression<Func<TProp>> propertySelector, FormElement attribute)
        {
            var memberExpression = propertySelector.Body as MemberExpression;
            if (memberExpression != null)
            {
                var allProperties = RetrieveProperties(memberExpression.Member.DeclaringType);

                var prop = allProperties.FirstOrDefault(p => p.Name == memberExpression.Member.Name);
                if (prop != null)
                {
                    DisplayProperties[prop] = attribute;
                }
            }
        }

        private PropertyInfo[] RetrieveProperties(Type entityType)
        {
            var props = AllProperties.SafeDictionaryValue<Type, PropertyInfo[], PropertyInfo[]>(entityType);
            if (props == null)
            {
                props = entityType.GetTypeInfo().DeclaredProperties.ToArray();
                AllProperties[entityType] = props;
            }
            return props;
        }

        public object Convert(object value, Type targetType, object parameter,
#if !NETFX_CORE|| DROID || DESKTOP
            CultureInfo culture
#else
 string language
#endif
            )
        {
#if NETFX_CORE
            if (DesignMode.DesignModeEnabled)
            {
                return new[]
                {
                    new FormProperty {Name="Name",Form=new FormElement(InputType.Text)}
                };

            }
#endif


            var entity = value as BaseEntityData;
            if (entity == null) return value;

            var props = entity.GetType().GetTypeInfo().DeclaredProperties;
            var tups = (from p in props
                        let name = p.Name
                        let custom = DisplayProperties.SafeDictionaryValue<PropertyInfo,FormElement,FormElement>(p)
                        where custom != null
                        select BuildFormProperty(name, custom, entity, p)).ToArray();
            return tups;
        }

        private object BuildFormProperty(string name, FormElement custom, BaseEntityData entity, PropertyInfo propertyInfo)
        {
            if (custom.Input != InputType.Dropdown)
            {
                return new FormProperty { Name = name, Form = custom, Entity = entity, PropertyInfo = propertyInfo };
            }
            else
            {
                //var fe = custom.GetType().GenericTypeArguments;
                //if (fe == null || fe.Length == 0) return null;
                //var gt = typeof(FormPropertySelector<>).MakeGenericType(fe[0]);
                //var fp = Activator.CreateInstance(gt) as FormProperty;

                var fe = custom as IFormSelector;

                var fp = new FormPropertySelector();
                fp.Name = name;
                fp.Form = custom;
                fp.Entity = entity;
                fp.PropertyInfo = propertyInfo;
                BuildSelectionValues(fp, fe);
                return fp;
            }
        }

        private void BuildSelectionValues(FormPropertySelector fp, IFormSelector fe)
        {
            Task.Run(async () =>
            {
                var ui = ServiceLocator.Current.GetInstance<IUIContext>();
                var values = await fe.BuildSelection();
                await ui.RunOnUIThreadAsync(async () => { fp.FormSelectionValues = values; });
            });


        }

        public object ConvertBack(object value, Type targetType, object parameter,
#if !NETFX_CORE || DROID|| DESKTOP
 CultureInfo culture
#else
            string language
#endif
            )
        {
            return value;
        }
    }

    public class FormProperty : NotifyBase
    {
        public string Name { get; set; }
        public FormElement Form { get; set; }

        public BaseEntityData Entity { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public object FormValue
        {
            get
            {
                var val = PropertyInfo.GetValue(Entity);
                return val;
            }
            set
            {
                PropertyInfo.SetValue(Entity, value);
            }
        }

    }

    public class FormPropertySelector : FormProperty
    {
        private IFormWrappedEntity[] formValues;
        public IFormWrappedEntity[] FormSelectionValues
        {
            get
            {
                
                return formValues;

            }
            set
            {
                formValues = value;
                OnPropertyChanged();
            }
        }

        //private async void UpdateFormValues()
        //{
        //    var dataService = ServiceLocator.Current.GetInstance<IDataService>();
        //    var dataTable = dataService.MobileService.GetSyncTable<TSelector>();
        //    FormSelectionValues = await dataTable.ToListAsync();
        //}

        
    }

    public class PropertyFormDisplayConverter : FormDisplayConverter
    {
        protected override void DefineProperties()
        {
            RealEstateProperty rep = null;

            DefineForm(() => rep.Address, new FormElement(InputType.Text));
            DefineForm(() => rep.PropertyTypeId, new FormSelectorElement<PropertyType, string>(InputType.Dropdown, pt => pt.Name, pt => pt.Id));
        }
    }

    public class FormValueWrapper<TEntity, TValue> : IFormWrappedEntity where TEntity : BaseEntityData
    {
        public TEntity Entity { get; set; }

        public FormSelectorElement<TEntity, TValue> Form { get; set; }

        public string Display
        {
            get { return Form.Display(Entity); }
        }
    }

    public interface IFormWrappedEntity
    {
    }


    public enum InputType
    {
        Text,
        Dropdown
    }


    public class FormSelectorElement<TSelection, TValue> : FormElement, IFormSelector where TSelection : BaseEntityData
    {
        public InputType Input { get; set; }
        public Func<TSelection, string> Display { get; set; }
        public Func<TSelection, TValue> Value { get; set; }



        public FormSelectorElement(InputType input,
            Func<TSelection, string> display,
            Func<TSelection, TValue> value)
            : base(input)
        {
            Input = input;
            Display = display;
            Value = value;
        }

        public async Task<IFormWrappedEntity[]> BuildSelection()
        {
            var dataService = ServiceLocator.Current.GetInstance<IDataService>();
            var dataTable = dataService.MobileService.GetSyncTable<TSelection>();
            var selection = await dataTable.ToListAsync();
            return (from s in selection
                    select new FormValueWrapper<TSelection, TValue> { Entity = s, Form = this }).OfType < IFormWrappedEntity>().ToArray();
        }
    }

    public interface IFormSelector

    {
        Task<IFormWrappedEntity[]> BuildSelection();
    }

    public class FormElement
    {
        public InputType Input { get; set; }

        public FormElement(InputType input)
        {
            Input = input;
        }
    }
}
