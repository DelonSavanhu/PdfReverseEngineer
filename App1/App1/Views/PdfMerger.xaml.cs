using App1.libs;
using App1.Models;
using App1.Services;
using App1.Services.Engines;
using App1.Services.Interfaces;
using Newtonsoft.Json;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Syncfusion.Pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PdfMerger : ContentPage
    {
        IDownloader downloader = DependencyService.Get<IDownloader>();

        Misc misc = new Misc();
        public ObservableCollection<string> Items { get; set; }
        public List<string> request = new List<string>();
        ObservableCollection<Document> request2 { get; set; }
        string path = "";
        public PdfMerger()
        {
            InitializeComponent();
            Title = "Combine PDF files.";
            this.Init();
        }
        public async void Init()
            {
            try
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
                if (Device.RuntimePlatform == Device.iOS)
                {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                }
                else if (Device.RuntimePlatform == Device.Android)
                {
                    path = misc.GetPath();//System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                }
            }
            catch (Exception ex) { }
        }

        async void Handle_ItemTapped(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            var selectedItem = (Document)e.ItemData;
            bool answer =await DisplayAlert("Remove from list.", "Are you sure you want to remove "+ selectedItem.name+"?", "Yes", "No");
            if (answer) {
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
                byte[] data = fileData.DataArray;
                string name = fileData.FileName;
                //string filePath = fileData.FilePath;
                string filePath = misc.SaveByteArrayToFileWithFileStream(fileData.DataArray, name);
                if (!misc.AllowedTypes(Path.GetExtension(filePath), new string[] { ".pdf" }))
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
                if (this.MergerDocuments(request))
                {
                    await DisplayAlert("All Done!", "Your file has been generated.", "Ok");
                    this.Init();
                    await Navigation.PushAsync(new MyDocuments());
                }
                else
                {
                    throw new Exception("Could not generate file");
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

        private bool MergerDocuments(List<string> paths)
        {
            //Loads the file as stream
            List<Stream> streams = new List<Stream>();
            for (int x=0; x < paths.Count(); x++)
            {
                Stream stream1 = File.OpenRead(paths[x]);// typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("Sample.Assets.file1.pdf");
                streams.Add(stream1);
            }
            //Creates a PDF stream for merging
            Stream[] source = streams.ToArray();//{ stream1, stream2 };
            //Create a new PDF document
            PdfDocument document = new PdfDocument();
            //Merge the documents
            PdfDocumentBase.Merge(document, source);
            //Save the PDF document to stream
            MemoryStream stream = new MemoryStream();
            document.Save(stream);
            //Close the documents
            document.Close(true);
            string timeStamp = DateTime.Now.ToFileTime().ToString();
            misc.RemoveAdhoc();
            return misc.CopyStream(stream, misc.GetPath()+ "/merged_" + timeStamp+".pdf");
        }
    }
}
