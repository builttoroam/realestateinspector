using System;

namespace BuiltToRoam
{
    public interface ILogService
    {
        void Debug(string message);

        void Exception(string message, Exception ex);
    }
}