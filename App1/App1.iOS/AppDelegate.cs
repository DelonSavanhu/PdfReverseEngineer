using Syncfusion.XForms.iOS.Buttons;
using Syncfusion.ListView.XForms.iOS;
using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Xamarin.Forms;
using App1.Services;
using Plugin.Toasts;
using UserNotifications;

namespace App1.iOS
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
            Google.MobileAds.MobileAds.Configure("ca-app-pub-9511268744828643~4450184935");
            global::Xamarin.Forms.Forms.Init();
            SfButtonRenderer.Init();
            SfListViewRenderer.Init();
            //DependencyService.Register<IPathService, PathService>();
            DependencyService.Register<IToastNotificator, ToastNotification>();
            LoadApplication(new App());
            ToastNotification.Init();
            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert
                                                      | UNAuthorizationOptions.Badge
                                                      | UNAuthorizationOptions.Sound,
                                                      (granted, error) =>
                                                      {
                                                          // Do something if needed
                                                      });
            return base.FinishedLaunching(app, options);
        }
    }
}
