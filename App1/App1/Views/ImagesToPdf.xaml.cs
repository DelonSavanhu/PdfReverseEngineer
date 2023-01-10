using App1.Models;
using App1.Services.Engines;
using App1.Services.Interfaces;
using Newtonsoft.Json;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using App1.libs;
using App1.Services;
using Plugin.Toasts;
using System.Collections.Generic;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Xamarin.Essentials;
using static Xamarin.Essentials.AppleSignInAuthenticator;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace App1.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ImagesToPdf : ContentPage
    {
        IDownloader downloader = DependencyService.Get<IDownloader>();
        Misc misc = new Misc();
        public ObservableCollection<string> Items { get; set; }
        public List<string> request = new List<string>();
        
        ObservableCollection<Document> request2  { get; set; }
        public ImagesToPdf ()
		{
			InitializeComponent ();
            Title = "Combine images into one PDF file.";
            this.Init();
        }
        public async void Init()
        {
            Items = new ObservableCollection<string>();
            request2 = new ObservableCollection<Document>();
            MyListView.ItemsSource = request2;
            MyListView.ItemTapped += Handle_ItemTapped;
            loader.IsRunning = true;
            loader.IsVisible = false;
            if (Items != null && Items.Count > 0)
            {
                SendToServer.IsVisible = true;
            }
            else
            {
                SendToServer.IsVisible = false;
            }

            BaseService bs = new BaseService();
            List<Config> configs = await bs.GetConfigs();
            foreach (Config c in configs)
            {
                if (c.name.Equals("ads") && c.value.Equals("1"))// admin has set to show ads on devices
                {
                    if (Device.RuntimePlatform == Device.Android)
                        adMobView.AdUnitId = "ca-app-pub-9511268744828643/1991257149";
                    else if (Device.RuntimePlatform == Device.iOS)
                        adMobView.AdUnitId = "ca-app-pub-9511268744828643/4042705410";

                }
            }

        }

        async void Handle_ItemTapped(object sender,Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            var selectedItem = (Document)e.ItemData;
            bool answer = await DisplayAlert("Remove from list.", "Are you sure you want to remove " + selectedItem.name+ "?", "Yes", "No");
            if (answer)
            {
                var q = request2.IndexOf(selectedItem);

                int index = q;
                Items.RemoveAt(index);
                request.RemoveAt(index);// remoce from list of items to be sent to the server
                request2.RemoveAt(index);
            }
            //Deselect Item
            MyListView.SelectedItem = null;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                
                  FileData fileData = new FileData();
                  fileData = await CrossFilePicker.Current.PickFile();

                  string name = fileData.FileName;
                  //string filePath = fileData.FilePath;
                  string filePath = misc.SaveByteArrayToFileWithFileStream(fileData.DataArray, name);
                  //check if filetype is allowed
                  if(!misc.AllowedTypes(Path.GetExtension(filePath),new string[] { ".png", ".jpeg",".jpg",".tiff",".gif" }))
                  {
                      misc.ShowNotification("Invalid file", "Please select a valid file.", false, "error.png");
                      throw new CustomException("Please select a valid image file.");
                  }

                  Items.Add(name);
                  request.Add(filePath);

                  FileInfo oFileInfo = new FileInfo(filePath);


                  request2.Add(new Document
                  {
                       date=oFileInfo.CreationTime.ToString("dd MMM HH:mm"),
                       name=oFileInfo.Name,
                       path=filePath,
                       size= Math.Round(misc.ConvertBytesToMegabytes(oFileInfo.Length), 2).ToString() + " MB",
                       type=oFileInfo.Extension,
                       img=misc.GetIcon(filePath)
                  });


                  if (request2 != null && request2.Count > 0)
                  {
                      SendToServer.IsVisible = true;
                  }
                  else
                  {
                      SendToServer.IsVisible = false;
                  }

                 
                //PickOptions options = new PickOptions { PickerTitle = "Hello" };
                //var result = await this.PickAndShow(options);


            }
            catch (CustomException ex)
            {
                //await DisplayAlert("Error",ex.Message,"Ok");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
        async Task<FileResult> PickAndShow(PickOptions options)
        {
            try
            {
                var result = await FilePicker.PickAsync(options);
                if (result != null)
                {
                    //Text = $"File Name: {result.FileName}";
                    if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                    {
                        var stream = await result.OpenReadAsync();
                        //Image = ImageSource.FromStream(() => stream);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }

            return null;
        }
        private async void Merge_Clicked(object sender, EventArgs e)
        {
            try
            {
                loader.IsVisible = true;
                await Task.Delay(500);
                if (this.MergerImages(request))
                {
                    await DisplayAlert("All Done!", "Your files have been generated.", "Ok");
                    this.Init();
                    await Navigation.PushAsync(new MyDocuments());
                }
                else
                {
                    throw new Exception("Failed to generate that file.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                loader.IsVisible = false;
                await DisplayAlert("Error occured!", "Please make sure you have internet and no duplicate files.", "Close");
            }

        }

        private EventArgs OnFileDownloaded(object sender, DownloadEventArg e)
        {
            if (e.FileSaved)
            {
                DisplayAlert("XF Downloader", "File Saved Successfully", "Close");
            }
            else
            {
                DisplayAlert("XF Downloader", "Error while saving the file", "Close");
            }
            return e;
        }

        private bool MergerImages(List<string> paths)
        {
            try
            {
                //Loads the file as stream
                List<Stream> streams = new List<Stream>();
                PdfDocument doc = new PdfDocument();

                for (int x = 0; x < paths.Count(); x++)
                {
                    //Add a page to the document
                    PdfPage page = doc.Pages.Add();
                    //Create PDF graphics for the page
                    PdfGraphics graphics = page.Graphics;
                    //Load the image as stream
                    Stream stream1 = File.OpenRead(paths[x]);// typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("Sample.Assets.file1.pdf");
                    PdfBitmap image = new PdfBitmap(stream1);
                    //Draw the image
                    graphics.DrawImage(image, 0, 0);
                    //streams.Add(stream1);
                }
                MemoryStream stream = new MemoryStream();
                doc.Save(stream);
                doc.Close(true);
                string timeStamp = DateTime.Now.ToFileTime().ToString();
                misc.RemoveAdhoc();
                return misc.CopyStream(stream, misc.GetPath() + "/merged_" + timeStamp + ".pdf");
            }
            catch (Exception e)
            {
                throw;
            }
        }


    }
}