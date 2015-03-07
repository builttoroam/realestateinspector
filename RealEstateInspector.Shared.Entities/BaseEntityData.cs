#if SERVICE
using Microsoft.WindowsAzure.Mobile.Service;
#else
using Microsoft.WindowsAzure.MobileServices;
#endif
using System;

namespace RealEstateInspector.Shared.Entities
{
    public class BaseEntityData
#if SERVICE
       : EntityData{}
#else
    {
        public string Id { get; set; }

        [CreatedAt]
        public DateTimeOffset CreatedAt { get; set; }

        [UpdatedAt]
        public DateTimeOffset UpdatedAt { get; set; }

        [Version]
        public string Version { get; set; }

    }
#endif

}
