using System;
using BuiltToRoam;
using BuiltToRoam.Mobile;
using MetroLog;
using RealEstateInspector.Core;

namespace RealEstateInspector.Shared.Client
{
    public class LogService : ILogService
    {
        public ILogWriterService Writer { get; set; }

        public ILogger Logger { get; set; }

        public LogService(ILogWriterService writer)
        {
            Writer = writer;
            var target = new MobileServiceTarget(Writer);

            LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Debug, target);

            Logger = LogManagerFactory.DefaultLogManager.GetLogger("Default");
        }

        public void Debug(string message)
        {
            Logger.Debug(message);
        }

        public void Exception(string message, Exception ex)
        {
            Logger.Error(message, ex);
        }
    }
}