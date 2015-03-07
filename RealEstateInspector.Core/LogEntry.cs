using System;
using RealEstateInspector.Shared.Entities;

namespace RealEstateInspector.Core
{
    public class LogEntry : BaseEntityData
    {
        public DateTime LogDateTime { get; set; }
        public string Entry { get; set; }
    }
}