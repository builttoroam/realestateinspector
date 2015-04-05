using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuiltToRoam.Communication
{
    public interface ISignalR
    {
        Task<ICommunicationHub> Connect<THub>(string endpointUri, IDictionary<string,string> headers=null);
    }
}