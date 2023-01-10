using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections;
using Plugin.CurrentActivity;
using App1.Droid.Services;
using App1.Services;
using Xamarin.Forms;
using Android;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Plugin.Toasts;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using App1.Services.Interfaces;
using System.IO;
using App1.libs;
//using Android.Gms.Ads;

[assembly: MetaData("com.google.android.gms.ads.ca-app-pub-9511268744828643~6313645530", Value = "ca-app-pub-9511268744828643~6313645530")]
namespace App1.Droid
{
    [Activity(Label = "Delon PDF Util", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //try
            //{
                TabLayoutResource = Resource.Layout.Tabbar;
                ToolbarResource = Resource.Layout.Toolbar;

                base.OnCreate(savedInstanceState);
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage }, 114);
                //ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, 5);

                this.CheckAppPermissions();
                //MobileAds.Initialize(ApplicationContext, "ca-app-pub-9511268744828643/1991257149");
                global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
                CrossCurrentActivity.Current.Init(this, savedInstanceState);
                DependencyService.Register<IPathService, PathService>();
                DependencyService.Register<IPDFOpen, PDFSaveAndOpen>();
                DependencyService.Register<IToastNotificator, ToastNotification>();
                //
                ToastNotification.Init(this);
                Syncfusion.XForms.Android.PopupLayout.SfPopupLayoutRenderer.Init();
                LoadApplication(new App());
            /*}
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }*/
        }
         void CheckAppPermissions()
        {
        
            /*if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == (int)Permission.Granted)
            {
            }
            else
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, 1);
                // Camera permission is not granted. If necessary display rationale & request.
            }*/
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted && ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == (int)Permission.Granted)
            {
                // We have permission.
            }
            else
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage }, 114);
                
            }
        }
    }
}