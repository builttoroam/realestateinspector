using System;

namespace BuiltToRoam.Communication
{
    public interface ICommunicationHub:IDisposable
    {
        string ConnectionId { get; }

        IDisposable Register<TMessageType>(string eventName, Action<TMessageType> handler);
    }
}