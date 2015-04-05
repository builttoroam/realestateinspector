using System;
using System.Collections.Generic;
using System.Text;
using BuiltToRoam.Mobile;

//#if SERVICE
//using realestateinspectorService.DataObjects;
//#endif

namespace RealEstateInspector.Shared.Entities
{
    //public enum InputType
    //{
    //    Text,
    //    Dropdown
    //}

    //[AttributeUsage(AttributeTargets.Property)]
    //public class DisplayAttribute : Attribute
    //{

    //}


    //[AttributeUsage(AttributeTargets.Property)]
    //public class FormAttribute:Attribute
    //{
    //    public InputType Input { get; set; }
    //    public Type SelectType { get; set; }


    //    public FormAttribute(InputType input, 
    //        Type selectType=null)
    //    {
    //        Input = input;
    //        SelectType = selectType;
    //    }
    //}


    public class RealEstateProperty : BaseEntityData
    {
      //  [Form(InputType.Text)]
        public string Address { get; set; }

       // [Form(InputType.Dropdown,typeof(PropertyType))]
        public string PropertyTypeId { get; set; }
        public PropertyType PropertyType { get; set; }

        public virtual ICollection<Inspection> Inspections { get; set; }
    }

    public class Inspection : BaseEntityData
    {
        public string InspectedBy { get; set; }

        public string RealEstatePropertyId { get; set; }
        public RealEstateProperty RealEstateProperty { get; set; }

    }

    public class PropertyType : BaseEntityData
    {
     //   [Display]
        public string Name { get; set; }
    }
}
