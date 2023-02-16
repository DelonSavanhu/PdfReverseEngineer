using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App1.Services;
using Java.Security.Cert;

[assembly: Dependency("App1.Droid.Services.PathService", LoadHint.Always)]
namespace App1.Droid.Services
{
    public class PathService : IPathService
    {
        public PathService()
        {
            MainActivity activity = new MainActivity();          
        }
        public string InternalFolder
        {
            get
            {
                return Android.App.Application.Context.FilesDir.AbsolutePath;
            }
        }

        public string PublicExternalFolder
        {
            //Android.OS.Environment.ExternalStorageDirectory.AbsolutePath 
            get
            {
                return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;//Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            }
        }

        public string PrivateExternalFolder
        {
            get
            {
                return Application.Context.GetExternalFilesDir(null).AbsolutePath;
            }
        }

        public int checkVersion()
        {
            int version = 0;

            var sdk = Build.VERSION.Sdk;
            if (Int32.TryParse(sdk, out version))
            {
                return version;
            }
            else
            {
                return version;
            }

        }

    }
}