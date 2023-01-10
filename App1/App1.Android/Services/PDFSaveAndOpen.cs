using Android;
using Android.Content;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Webkit;
using Java.IO;
using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Xamarin.Forms;
using App1.Services.Interfaces;
using Android.OS;
using Xamarin.Essentials;
using WordPdf;

[assembly: Dependency(typeof(App1.Droid.Services.PDFSaveAndOpen))]
namespace App1.Droid.Services
{
   public  class PDFSaveAndOpen:IPDFOpen
    {
        public async Task SaveAndView(string fileName, String contentType, PDFOpenContext context)
        {
            string exception = string.Empty;
            string root = null;

            Java.IO.File file = new Java.IO.File(fileName);
            if (file.Exists())
            {
                string extension = MimeTypeMap.GetFileExtensionFromUrl(Android.Net.Uri.FromFile(file).ToString());
                string mimeType = MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension);
                
                Intent intent = new Intent(Intent.ActionView);
               
                /*if (Build.VERSION.SdkInt >= Build.VERSION_CODES.N)
                {
                    Android.Net.Uri uri = FileProvider.GetUriForFile(Android.App.Application.Context, Android.App.Application.Context.PackageName + ".provider", file);
                    intent.SetDataAndType(uri, mimeType);
                    intent.SetFlags(ActivityFlags.GrantReadUriPermission);
                    
                }
                else
                {
                    Android.Net.Uri uri = Android.Net.Uri.FromFile(file);
                    intent.SetDataAndType(uri, mimeType);
                }*/


                //intent.SetDataAndType(uri, mimeType);

                intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);


                switch (context)
                {
                    case PDFOpenContext.InApp:
                        Android.App.Application.Context.StartActivity(intent);
                        break;
                    case PDFOpenContext.ChooseApp:
                        var chooserIntent = Intent.CreateChooser(intent, "Open File: " + file.Name + "?");
                        chooserIntent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);
                        Android.App.Application.Context.StartActivity(chooserIntent);
                        ///Android.App.Application.Context.StartActivity(Intent.CreateChooser(intent,"Open File: "+ file.Name +"?"));
                        break;
                    default:
                        break;
                }
            }
        }

        public void WordPdf(string filename,string path)
        {
            WordToPdf wtp = new WordToPdf();
            wtp.CreateDoc(filename, path);
        }

    }
}