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
using Xamarin.Essentials;
using Xamarin.Forms.PlatformConfiguration;
//using Android.Gms.Ads;

[assembly: MetaData("com.google.android.gms.ads.ca-app-pub-9511268744828643~6313645530", Value = "ca-app-pub-9511268744828643~6313645530")]
namespace App1.Droid
{
    [Activity(Label = "Delon PDF Util", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                TabLayoutResource = Resource.Layout.Tabbar;
                ToolbarResource = Resource.Layout.Toolbar;
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                base.OnCreate(savedInstanceState);
                //this.checkVersion();

                //ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage }, 114);
                //ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, 5);

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
                this.CheckAppPermissions();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        async void checkVersion()
        {
          /*  int version = 0;

            var sdk=Build.VERSION.Sdk;
            if(Int32.TryParse(sdk, out version))
            {
                if (version > 28)
                {
                    
                    await App.Current.MainPage.DisplayAlert("Sorry", "This app does not support the android version you have.", "Exit");
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Sorry", "This app does not support the android version you have.", "Exit");
            }*/

        }
        async void CheckAppPermissions()
        {
            try
            {
                if (Permissions.ShouldShowRationale<Permissions.StorageRead>())
                {
                    // Prompt the user with additional information as to why the permission is needed
                }
                var status =await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.StorageRead>();
                }
                
                var status2 =await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                if (status2 != PermissionStatus.Granted)
                {
                    status2 =await Permissions.RequestAsync<Permissions.StorageWrite>();
                }
                var status3 = await Permissions.CheckStatusAsync<Permissions.Media>();
                if (status3 != PermissionStatus.Granted)
                {
                    status3 = await Permissions.RequestAsync<Permissions.Media>();
                }
                var status4 = await Permissions.CheckStatusAsync<Permissions.Photos>();
                if (status4 != PermissionStatus.Granted)
                {
                    status4 = await Permissions.RequestAsync<Permissions.Photos>();
                }

                /*                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
                                {
                                }
                                else
                                {
                                    ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.WriteExternalStorage,Manifest.Permission.ReadExternalStorage,Manifest.Permission.ReadMediaImages }, 114);
                                    // Camera permission is not granted. If necessary display rationale & request.
                                }
                                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == (int)Permission.Granted)
                                {
                                }
                                else
                                {
                                    ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, 1);
                                    // Camera permission is not granted. If necessary display rationale & request.
                                }
                                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted && ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == (int)Permission.Granted)
                                {
                                    // We have permission.
                                }
                                else
                                {
                                    ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage,Manifest.Permission.ReadMediaImages }, 114);

                                }*/
                /*if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == (int)Permission.Granted && ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == (int)Permission.Granted)
                {
                    // We have permission.
                }
                else
                {
                    ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.ReadExternalStorage }, 115);

                }*/
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            try
            {
                Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
            catch(Exception e)
            {

            }
        }

    }
}