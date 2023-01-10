using Syncfusion.SfImageEditor.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
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
using Plugin.Media.Abstractions;
using Plugin.Media;
using App1.libs;
using App1.Services;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf;

namespace App1.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ScanToPdf : ContentPage
    {
        string path = "";
        Misc misc = new Misc();
        public ObservableCollection<string> Items { get; set; }
        public List<string> request = new List<string>();
        string toBeRemoved = "";
        ObservableCollection<Document> request2 { get; set; }
        Syncfusion.SfImageEditor.XForms.SfImageEditor editor = new Syncfusion.SfImageEditor.XForms.SfImageEditor();
        public ScanToPdf ()
		{
			InitializeComponent ();
            Title = "Scan to PDF";
            this.Init();
        }

        public async void Init()
        {
            editor.ImageSaved += editor_ImageSaved;
            Items = new ObservableCollection<string>();
            request2 = new ObservableCollection<Document>();
            MyListView.ItemsSource = request2;
            MyListView.ItemTapped += Handle_ItemTapped;
            loader.IsRunning = true;
            loader.IsVisible = false;
            if (Device.RuntimePlatform == Device.iOS)
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);              
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                path = misc.GetPath(); // System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            }
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

        async void Handle_ItemTapped(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            var selectedItem = (Document)e.ItemData;
            bool answer = await DisplayAlert("Edit Image.", "Are you sure you want to edit " + selectedItem.name + "?", "Yes", "No");
            if (answer)
            {
                var q = request2.IndexOf(selectedItem);
                int index = q;
                toBeRemoved = request[q].ToString();
                ImageSource imagesource=ImageSource.FromFile(toBeRemoved);
                editor.Source = imagesource; 
                editor.RotatableElements = ImageEditorElements.Text;
                //Content = editor;
                mainGrid.Children.Add(editor);
                
            }
            //Deselect Item
            MyListView.SelectedItem = null;
        }

        private async void Button_Clicked(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            try
            {
                FileData fileData = new FileData();
                fileData = await CrossFilePicker.Current.PickFile();
                byte[] data = fileData.DataArray;
                string name = fileData.FileName;
                //string filePath = fileData.FilePath;
                string filePath = misc.SaveByteArrayToFileWithFileStream(fileData.DataArray, name);
                if (!misc.AllowedTypes(Path.GetExtension(filePath), new string[] { ".png", ".jpeg", ".jpg", ".tiff", ".gif" }))
                {
                    misc.ShowNotification("Invalid file", "Please select a valid file.", false, "error.png");
                    throw new CustomException("Please select a valid image file.");
                }

                Items.Add(name);
                request.Add(filePath);


                FileInfo oFileInfo = new FileInfo(filePath);


                request2.Add(new Document
                {
                    date = oFileInfo.CreationTime.ToString("dd MMM HH:mm"),
                    name = oFileInfo.Name,
                    path = filePath,
                    size = Math.Round(misc.ConvertBytesToMegabytes(oFileInfo.Length), 2).ToString() + " MB",
                    type = oFileInfo.Extension,
                    img = misc.GetIcon(filePath)
                });


                if (request2 != null && request2.Count > 0)
                {
                    SendToServer.IsVisible = true;
                }
                else
                {
                    SendToServer.IsVisible = false;
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private async void Merge_Clicked(object sender, EventArgs e)
        {
            try
            {
                loader.IsVisible = true;
                await Task.Delay(500);
                if (this.MergerImages(request))
                {
                    misc.RemoveAdhoc();
                    await DisplayAlert("All Done!", "Your files have been generated.", "Ok");
                    this.Init();
                    await Navigation.PushAsync(new MyDocuments());
                }
                else
                {
                    throw new Exception("Failed to generate that file.");
                }
                /*loader.IsVisible = true;
                Pdf dx = new Pdf();
                var re = new Models.Image_Request
                {
                    filename = request
                };
                await Task.Delay(500);
                var response = Task.Run(async () => await dx.ImageToPDF(re)).Result;



                ServerResponse dt = JsonConvert.DeserializeObject<ServerResponse>(response.response);
                FileDownloader fd = new FileDownloader();
                string r = "";
                
                if (Device.RuntimePlatform == Device.iOS)
                {
                    r = await fd.DownloadFileAsync(dt.url, path);
                }
                else if (Device.RuntimePlatform == Device.Android)
                {
                    r = await fd.DownloadFileAsync(dt.url, path);
                    Console.WriteLine(r);
                }

                loader.IsVisible = false;
                Items = new ObservableCollection<string>();//empty the list
                MyListView.ItemsSource = Items;
                if (File.Exists(r))
                {
                    await DisplayAlert("All Done!", "Your files have been generated.", "Ok");
                    this.Init();
                    await Navigation.PushAsync(new MyDocuments());
                }
                else
                {
                    await DisplayAlert("Oops!", "We failed to generate your file.", "Ok");
                }*/
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                loader.IsVisible = false;
                await DisplayAlert("Error occured!", "Please make sure you have internet and no duplicate files.", "Close");
            }

        }

        private void editor_ImageSaved(object sender, ImageSavedEventArgs args)
        {
            string savedLocation = args.Location; // You can get the saved image location with the help of this argument                    
            var q = request.IndexOf(toBeRemoved);
            string filename = Path.GetFileName(savedLocation);
            int index = q;
            //Items.Remove(selectedItem);
            Items.RemoveAt(index);
            Items.Add(filename);
            request.RemoveAt(index);// remoce from list of items to be sent to the server
            request.Add(savedLocation);
            mainGrid.Children.Remove(editor);
        }
        private async void takePic_ImageSaved(object sender, EventArgs args)
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "No camera avaialble.", "OK");
                return;
            }
            var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() {               
                DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Rear,
                PhotoSize = PhotoSize.Medium
            });
            loader.IsVisible = true;
            if (photo != null)
            {
                try
                {
                    await Task.Delay(500);
                    string fileName = Path.GetFileName(photo.Path);

                    string name = Path.GetFileName(photo.Path);
                    Items.Add(name);
                    request.Add(photo.Path);
                    FileInfo oFileInfo = new FileInfo(photo.Path);
                    request2.Add(new Document
                    {
                        date = oFileInfo.CreationTime.ToString("dd MMM HH:mm"),
                        name = oFileInfo.Name,
                        path = photo.Path,
                        size = Math.Round(misc.ConvertBytesToMegabytes(oFileInfo.Length), 2).ToString() + " MB",
                        type = oFileInfo.Extension,
                        img = misc.GetIcon(photo.Path)
                    });
                    if (request2 != null && request2.Count > 0)
                    {
                        SendToServer.IsVisible = true;
                    }
                    else
                    {
                        SendToServer.IsVisible = false;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            loader.IsVisible = false;
        }

        private bool MergerImages(List<string> paths)
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
            return misc.CopyStream(stream, misc.GetPath() + "/merged_" + timeStamp + ".pdf");
        }


    }
}