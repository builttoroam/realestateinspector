using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.WindowsAzure.MobileServices;
using RealEstateInspector.Core.ViewModels;
using RealEstateInspector.Shared.Client;

namespace RealEstateInspector.Desktop
{
    public partial class MainWindow : IWin32Window
    {

        public IntPtr Handle
        {
            get
            {
                var interopHelper = new WindowInteropHelper(this);
                return interopHelper.Handle;
            }
        }
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //var vm = new MainViewModel();
            //DataContext = vm;
        }

        
    }

   
}
