using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;

namespace BuiltToRoam
{
    public static class LogHelper
    {
        public static void Log<TEntity>(TEntity entity, [CallerMemberName] string caller = null)
        {
            var json = JsonConvert.SerializeObject(entity);
            Log(typeof(TEntity).Name + ": " + json, caller);
        }

        public static void Log(string message = null, [CallerMemberName] string caller = null)
        {
            try
            {
                InternalWriteLog("[" + caller + "] " + message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static void Log(this Exception ex, string message = null, [CallerMemberName] string caller = null)
        {
            try
            {
                Debug.WriteLine("Exception ({0}): {1}", caller, ex.Message);
                InternalWriteException(caller + ": " + message, ex);
            }
            catch (Exception ext)
            {
                Debug.WriteLine(ext.Message);
            }
        }


        private static ILogService logService;

        private static ILogService LogService
        {
            get
            {
                if (logService == null)
                {
                    logService = ServiceLocator.Current.GetInstance<ILogService>();

                }
                return logService;
            }
        }

        private static void InternalWriteLog(string message)
        {
            try
            {

                LogService.Debug(message);
            }
            catch (Exception ext)
            {
                Debug.WriteLine(ext.Message);
            }
        }


        private static void InternalWriteException(string message, Exception ex)
        {
            try
            {
                LogService.Exception(message, ex);
            }
            catch (Exception ext)
            {
                Debug.WriteLine(ext.Message);
            }
        }
    }
}