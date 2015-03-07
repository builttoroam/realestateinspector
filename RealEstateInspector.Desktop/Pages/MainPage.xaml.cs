using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RealEstateInspector.Core;
using RealEstateInspector.Core.ViewModels;
using RealEstateInspector.Shared.Client;

namespace RealEstateInspector.Desktop.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public MainViewModel CurrentViewModel
        {
            get { return DataContext as MainViewModel; }
        }


        private async void AuthenticateClick(object sender, RoutedEventArgs e)
        {
            var window = App.Current.MainWindow as MainWindow;

            var token = await AuthenticationHelper.Authenticate(window.Handle);

            await CurrentViewModel.DataService.Initialize(token);

            await CurrentViewModel.LoadPropertyData();
        }

        private async void AddPropertyClick(object sender, RoutedEventArgs e)
        {
            await CurrentViewModel.AddProperty();
        }

        private void GenerateReportClick(object sender, RoutedEventArgs e)
        {
            CurrentViewModel.GenerateReport();
        }
        private void NavigateClick(object sender, RoutedEventArgs e)
        {
            CurrentViewModel.GoToSecondPage();
        }

        //private async Task<string> ConnectToSignalR()
        //{
        //    var hubConnection = new HubConnection(MainViewModel.MobileService.ApplicationUri.AbsoluteUri);
        //    //if (user != null)
        //    //{
        //    //    hubConnection.Headers["x-zumo-auth"] = user.MobileServiceAuthenticationToken;
        //    //}
        //    //else
        //    //{
        //    hubConnection.Headers["x-zumo-application"] = MainViewModel.MobileService.ApplicationKey;
        //    //}
        //    IHubProxy proxy = hubConnection.CreateHubProxy("LongRunningFeedbackHub");
        //    await hubConnection.Start();


        //    //string result = await proxy.Invoke<string>("Send", "Hello World!");
        //    //var invokeDialog = new MessageDialog(result);
        //    //await invokeDialog.ShowAsync();

        //    proxy.On<string>("Progress",
        //        msg => Debug.WriteLine(msg));

        //    return hubConnection.ConnectionId;
        //}

        private async void WriteClick(object sender, RoutedEventArgs e)
        {
            await CurrentViewModel.WriteFile();
        }

        private async void ReadClick(object sender, RoutedEventArgs e)
        {
            await CurrentViewModel.ReadFile();
        }
    }
    public class WPFNavigationService : CoreNavigateService<Page>
    {
        protected override void NavigateToView(Type viewType)
        {
            (App.Current.MainWindow.Content as Frame).Navigate(new Uri("/Pages/" + viewType.Name + ".xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
