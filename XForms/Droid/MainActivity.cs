using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using RealEstateInspector.Core.ViewModels;
using RealEstateInspector.Shared.Client;
using Debug = System.Diagnostics.Debug;

namespace RealEstateInspector.XForms.Droid
{
    [Activity(Label = "RealEstateInspector.XForms", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationAgentContinuationHelper.SetAuthenticationAgentContinuationEventArgs(requestCode, resultCode, data);
        }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            RealEstateInspector.XForms.MainPage.AuthenticateRequested += Authenticate;
            LoadApplication(new App());
        }

        public async void Authenticate(object sender, EventArgs e)
        {
            var page = sender as RealEstateInspector.XForms.MainPage;
            await (page.BindingContext as MainViewModel).DataService.Initialize();

            var token = await AuthenticationHelper.Authenticate(this);
            Debug.WriteLine(token);
            await (page.BindingContext as MainViewModel).DataService.LoginAsync(token);
            await (page.BindingContext as MainViewModel).LoadPropertyData();
        }
    }
}

