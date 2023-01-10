using System;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms;
using UIKit;
using QuickLook;
using App1.Services.Interfaces;

[assembly: Dependency(typeof(App1.iOS.Services.PDFSaveAndOpen))]
namespace App1.iOS.Services
{
        public class PDFSaveAndOpen : IPDFOpen
        {
            //Method to save document as a file and view the saved document
            public async Task SaveAndView(string filename, string contentType, PDFOpenContext context)
            {
                //Get the root path in iOS device.
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string filePath = Path.Combine(path, filename);
                //Create a file and write the stream into it.
                FileStream fileStream = File.Open(filePath, FileMode.Open);
                //stream.Position = 0;
                //stream.CopyTo(fileStream);
                fileStream.Flush();
                fileStream.Close();

                //Invoke the saved document for viewing
                UIViewController currentController = UIApplication.SharedApplication.KeyWindow.RootViewController;
                while (currentController.PresentedViewController != null)
                    currentController = currentController.PresentedViewController;
                UIView currentView = currentController.View;

                QLPreviewController qlPreview = new QLPreviewController();
                QLPreviewItem item = new QLPreviewItemBundle(filename, filePath);
                qlPreview.DataSource = new PreviewControllerDS(item);

                currentController.PresentViewController(qlPreview, true, null);
            }

        public void WordPdf(string filename, string path)
        {
            throw new NotImplementedException();
        }
    }
    }