using System;

namespace RealEstateInspector.Core
{
    public interface ILogService
    {
        void Debug(string message);

        void Exception(string message, Exception ex);
    }
}