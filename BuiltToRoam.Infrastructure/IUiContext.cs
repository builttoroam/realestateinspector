using System;
using System.Threading.Tasks;

namespace BuiltToRoam
{
    public interface IUIContext
    {
        Task RunOnUIThreadAsync(Func<Task> action);
    }
}