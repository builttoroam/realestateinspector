using System;

namespace BuiltToRoam.Mobile
{
    public class LogEntry : BaseEntityData
    {
        public DateTime LogDateTime { get; set; }
        public string Entry { get; set; }
    }
}