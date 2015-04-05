using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BuiltToRoam.ViewModels
{
    public interface ICanGoBack
    {
        event EventHandler ClearPreviousViews;
        Task GoingBack(CancelEventArgs e);
    }
}