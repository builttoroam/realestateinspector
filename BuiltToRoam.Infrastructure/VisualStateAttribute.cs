using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuiltToRoam
{
    public class VisualStateAttribute : Attribute
    {
        public string VisualStateName { get; set; }

        public VisualStateAttribute(string visualStateName)
        {
            VisualStateName = visualStateName;
        }
    }
}
