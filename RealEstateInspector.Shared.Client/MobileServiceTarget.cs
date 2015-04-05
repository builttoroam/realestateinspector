using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BuiltToRoam.Mobile;
using MetroLog;
using MetroLog.Layouts;
using MetroLog.Targets;
using RealEstateInspector.Core;

namespace RealEstateInspector.Shared.Client
{
    public class MobileServiceTarget : Target
    {
        public ILogWriterService Writer { get; set; }
        public MobileServiceTarget(ILogWriterService writer)
            : base(new SingleLineLayout())
        {
            Writer = writer;
        }

        private bool InitCompleted { get; set; }
        protected async override Task<LogWriteOperation> WriteAsyncCore(LogWriteContext context, LogEventInfo entry)
        {
            try
            {
                if (!InitCompleted)
                {
                    await Writer.Initialize();
                    InitCompleted = true;
                }
                var log = new LogEntry { LogDateTime = DateTime.Now, Entry = entry.ToJson() };
                await Writer.LogTable.InsertAsync(log);
                return new LogWriteOperation(this, entry, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new LogWriteOperation(this, entry, false);
            }
        }
    }
}