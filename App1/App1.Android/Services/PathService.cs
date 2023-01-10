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
            get
            {
                return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            }
        }

        public string PrivateExternalFolder
        {
            get
            {
                return Application.Context.GetExternalFilesDir(null).AbsolutePath;
            }
        }
        
    }
}