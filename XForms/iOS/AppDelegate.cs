using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Foundation;
using RealEstateInspector.Core.ViewModels;
//using RealEstateInspector.Shared.Client;
using UIKit;

namespace RealEstateInspector.XForms.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            RealEstateInspector.XForms.MainPage.AuthenticateRequested += Authenticate;

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
        public async void Authenticate(object sender, EventArgs e)
        {
            var window= UIApplication.SharedApplication.KeyWindow;
var vc = window.RootViewController;

            var page = sender as RealEstateInspector.XForms.MainPage;
            //var token = await AuthenticationHelper.Authenticate(vc);
            //Debug.WriteLine(token);
            //(page.BindingContext as MainViewModel).LoadPropertyData(token);
        }
    }
}
