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

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using App1.Services;
using App1.libs;
using Syncfusion.SfPdfViewer.XForms;
using System.Text.RegularExpressions;
using Syncfusion.Pdf.Parsing;

namespace App1.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PdfToImages : ContentPage
    {
        IDownloader downloader = DependencyService.Get<IDownloader>();
        Misc misc = new Misc();
        public ObservableCollection<string> Items { get; set; }
        public List<string> request = new List<string>();
        ObservableCollection<Document> request2 { get; set; }

        public ObservableCollection<string> FormatList { get; set; }
        public PdfToImages ()
		{
			InitializeComponent ();
            Title = "Convert PDF file into images.";
            this.Init();
        }
        public async void Init()
        {
            Items = new ObservableCollection<string>();
            FormatList = new ObservableCollection<string>();
            FormatList.Add("PNG");
            FormatList.Add("JPEG");

            Extension.ItemsSource = FormatList;

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
        async void Handle_ItemTapped(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            var selectedItem = (Document)e.ItemData;
            bool answer = await DisplayAlert("Remove from list.", "Are you sure you want to remove " + selectedItem.name + "?", "Yes", "No");
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
                int selectedIndex = Int32.Parse(Extension.SelectedIndex.ToString());               
                if (selectedIndex == -1) //user did not select
                {
                    throw new CustomException("Please select a format.");
                }
                string ext = FormatList[selectedIndex];
                loader.IsVisible = true;
                await Task.Delay(500);
                int counter = 0;
                for (int x=0; x< request.Count; x++)
                {
                    if (this.PdfToImagesConversion(request[x],ext.ToLower()) > 0)
                    {
                        counter++;
                    }
                }
                if (counter > 0)
                {
                    misc.RemoveAdhoc();
                    await DisplayAlert("All Done!", "Your files have been generated.", "Ok");
                    this.Init();
                    await Navigation.PushAsync(new MyDocuments());
                }
                else
                {
                    throw new Exception("Failed to convert some files");
                }
            }
            catch(CustomException ex)
            {
                loader.IsVisible = false;
                await DisplayAlert("Error occured!", ex.Message, "Close");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                loader.IsVisible = false;
                await DisplayAlert("Error occured!", "Please make sure you have internet and no duplicate files.", "Close");
                await Navigation.PushAsync(new MyDocuments());
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

        private int PdfToImagesConversion(string path,string type)
        {
            Stream fileStream = File.OpenRead(path);
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(fileStream);

            int counter = 0; 
            SfPdfViewer pdfViewerControl = new SfPdfViewer();       
            
            pdfViewerControl.LoadDocument(fileStream);
            int pageCount = loadedDocument.PageCount;// getNumberOfPdfPages(path);//.PageCount;
            Stream[] streams = pdfViewerControl.ExportAsImage(0,pageCount-1);
            for (int x=0; x< streams.Length; x++) {
                int c=x + 1;
                    string pname = Path.GetFileName(path).Replace(".pdf", "") + "_" + c;
                    if (misc.CopyStream(streams[x], misc.GetPath() + "/"+pname+ "."+type))
                    {
                        counter++;
                    }
                }
                return counter;
        }

        public int getNumberOfPdfPages(string fileName)
        {
            using (StreamReader sr = new StreamReader(File.OpenRead(fileName)))
            {
                Regex regex = new Regex(@"/Type\s*/Page[^s]");
                MatchCollection matches = regex.Matches(sr.ReadToEnd());
                return matches.Count;
            }
        }
    }
}