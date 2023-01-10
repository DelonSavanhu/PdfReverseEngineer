using App1.libs;
using App1.Models;
using App1.Services;
using App1.Services.Engines;
using App1.Services.Interfaces;
using Newtonsoft.Json;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
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
	public partial class HtmlToPdf : ContentPage
    {
        IDownloader downloader = DependencyService.Get<IDownloader>();

        Misc misc = new Misc();
        public ObservableCollection<string> Items { get; set; }
        public ArrayList request = new ArrayList();
        ObservableCollection<Document> request2 { get; set; }
        public HtmlToPdf ()
		{
			InitializeComponent ();
            Title = "Convert HTML file to PDF.";
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
                string filePath = fileData.FilePath;


                if (!misc.AllowedTypes(Path.GetExtension(filePath), new string[] { ".html" }))
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
                Html dx = new Html();
                var re = new Models.Html_Request
                {
                    filename = request
                };
                await Task.Delay(500);
                var response = Task.Run(async () => await dx.HtmlToPDF(re)).Result;
                ServerResponse dt = JsonConvert.DeserializeObject<ServerResponse>(response.response);
                FileDownloader fd = new FileDownloader();
                string r = "";
                string path = "";
                if (Device.RuntimePlatform == Device.iOS)
                {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    r = await fd.DownloadFileAsync(dt.url, path);
                }
                else if (Device.RuntimePlatform == Device.Android)
                {
                    path = misc.GetPath();// System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                    r = await fd.DownloadFileAsync(dt.url, path);
                    Console.WriteLine(r);
                }

                loader.IsVisible = false;
                Items = new ObservableCollection<string>();//empty the list
                MyListView.ItemsSource = Items;
                //File f = new File(r);
                if (File.Exists(r))
                {
                    //let's unzip it
                    bool t = await dx.UnzipFileAsync(r, path);
                    if (t)
                    {//let's delete zip
                        File.Delete(r);
                    }
                    await DisplayAlert("All Done!", "Your files have been generated.", "Ok");
                    this.Init();
                    await Navigation.PushAsync(new MyDocuments());
                    //Navigation.RemovePage(this);
                }
                else
                {
                    await DisplayAlert("Oops!", "We failed to generate your file.", "Ok");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                loader.IsVisible = false;
                await DisplayAlert("Error occured!", "Please make sure you have internet and no duplicate files.", "Close");
            }

        }

    }
}