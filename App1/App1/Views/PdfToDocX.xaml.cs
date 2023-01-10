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
using System.Drawing;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using App1.Services;
using App1.libs;
using Syncfusion.Pdf.Parsing;
using Syncfusion.DocIO.DLS;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.DocIO;
using Syncfusion.Presentation;

namespace App1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PdfToDocX : ContentPage
    {
        IDownloader downloader = DependencyService.Get<IDownloader>();
        Misc misc = new Misc();
        public ObservableCollection<string> Items { get; set; }
        public List<string> request = new List<string>();
        ObservableCollection<Document> request2 { get; set; }

        public ObservableCollection<string> FormatList { get; set; }
        public PdfToDocX()
        {
            InitializeComponent();
            Items = new ObservableCollection<string>();
            Title = "PDF to Office Docscvxvdffdssssssssdxzc";
            this.Init();
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
        }
        private async void Init()
        {
            FormatList = new ObservableCollection<string>();
            FormatList.Add("DOCX");
            FormatList.Add("DOC");
            FormatList.Add("PPT");
            //FormatList.Add("XLXS");

            Extension.ItemsSource = FormatList;

            request2 = new ObservableCollection<Document>();
            MyListView.ItemsSource = request2;
            MyListView.ItemTapped += Handle_ItemTapped;

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
                loader.IsVisible = true;
                Pdf dx = new Pdf();
                int selectedIndex = Int32.Parse(Extension.SelectedIndex.ToString());
                await Task.Delay(500);
                if (selectedIndex == -1) //user did not select
                {
                    throw new CustomException("Please select the extension type.");
                }

                string ext = FormatList[selectedIndex];
           
                foreach (var p in request)
                {
                    if (ext.ToLower().Equals("docx") || ext.ToLower().Equals("doc")) {
                        this.PdfToWord(p, ext.ToLower());
                    }
                    else if (ext.ToLower().Equals("ppt"))
                    {
                        this.PdfToPowerpoint(p, "pptx");
                    }
                }
                await DisplayAlert("All Done!", "Your files have been generated.", "Ok");
                await Navigation.PushAsync(new MyDocuments());
            }
            catch (CustomException ex)
            {
                loader.IsVisible = false;
                await DisplayAlert("Error occured!", ex.Message, "Close");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                loader.IsVisible = false;
                await DisplayAlert("Error occured!", "Please make sure you have internet and no duplicate files.", "Close");
            }


        }

        public void PdfToWord(string path,string extension)
        {
            //Load the PDF document
            Stream stream1 = File.OpenRead(path);
            WordDocument document = new WordDocument(); //lets add the new document
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(stream1);

            TextLineCollection lineCollection = new TextLineCollection();

            for (int x=0; x< loadedDocument.PageCount; x++)
            {
                IWSection section = document.AddSection();
                section.PageSetup.Margins.All = 50f;
                PdfPageBase page = loadedDocument.Pages[x];
                
                string extractedText = page.ExtractText(true);
                string font = "Helvetica";
                float fontSize = 12;

                foreach (var tl in lineCollection.TextLine)
                {
                    Console.WriteLine(tl.FontName);
                    font = tl.FontName;
                    fontSize = tl.FontSize;                    
                }
                IWParagraph firstParagraph = section.AddParagraph();
                //Sets the paragraph's horizontal alignment as justify
                firstParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Justify;
                //Adds a text range into the paragraph
                IWTextRange firstTextRange = firstParagraph.AppendText(extractedText);
                //sets the font formatting of the text range
                //firstTextRange.CharacterFormat.Bold = true;
                firstTextRange.CharacterFormat.FontName = font;
                firstTextRange.CharacterFormat.FontSize = fontSize;
                
                var images = misc.PdfGetImages(path, x+1);
                var ImageList = images.ToList();
                               
                Console.WriteLine("\n\n\n" + extractedText);
                int imgNo = 0;
                foreach (var img in ImageList)
                {
                    imgNo++;
                    Console.WriteLine("\n\n\n Image #" + imgNo);
                    MemoryStream imgStream = new MemoryStream();
                    var byt = img.RawBytes.ToArray<byte>();
                    imgStream.Write(byt, 0, byt.Length);
                    misc.CopyStream(imgStream, misc.GetPath() + "/Page_" + x+1 + "_"+ imgNo + "_image.png");
                    IWPicture picture = firstParagraph.AppendPicture(imgStream);
                    //Specify the size of the picture
                    //picture.Height = img.;
                    //picture.Width = 100;

                    imgStream.Position = 0;
                    imgStream.Close();
                }
            }
           
            loadedDocument.Close(true);
            MemoryStream stream = new MemoryStream();
            document.Save(stream, Syncfusion.DocIO.FormatType.Docx);
            string timeStamp = DateTime.Now.ToFileTime().ToString();
            misc.CopyStream(stream, misc.GetPath() + "/generated_" + timeStamp + "."+extension);
            document.Close();
            misc.RemoveAdhoc();
        }

        public void PdfToPowerpoint(string path, string extension)
        {
            //Load the PDF document
            Stream stream1 = File.OpenRead(path);
           
            IPresentation document = Presentation.Create();

            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(stream1);

            TextLineCollection lineCollection = new TextLineCollection();

            for (int x = 0; x < loadedDocument.PageCount; x++)
            {                
                ISlide slide = document.Slides.Add(SlideLayoutType.TitleOnly);
                
                PdfPageBase page = loadedDocument.Pages[x];

                string extractedText = page.ExtractText(true);
                string font = "Helvetica";
                float fontSize = 12;

                foreach (var tl in lineCollection.TextLine)
                {
                    Console.WriteLine(tl.FontName);
                    font = tl.FontName;
                    fontSize = tl.FontSize;
                }

                IShape descriptionShape = slide.AddTextBox(53.22, 141.73, 874.19, 77.70);
                descriptionShape.TextBody.Text = extractedText;
                
                var images = misc.PdfGetImages(path, x + 1);
                var ImageList = images.ToList();

                int imgNo = 0;
                foreach (var img in ImageList)
                {
                    imgNo++;
                    MemoryStream imgStream = new MemoryStream();
                    var byt = img.RawBytes.ToArray<byte>();
                    imgStream.Write(byt, 0, byt.Length);
                    misc.CopyStream(imgStream, misc.GetPath() + "/Page_" + x + 1 + "_" + imgNo + "_image.png");
                    slide.Shapes.AddPicture(imgStream, 499.79, 238.59, 364.54, 192.16);
                    imgStream.Position = 0;
                    imgStream.Close();
                }
            }

            loadedDocument.Close(true);
            MemoryStream stream = new MemoryStream();
            document.Save(stream);
            string timeStamp = DateTime.Now.ToFileTime().ToString();
            misc.CopyStream(stream, misc.GetPath() + "/generated_" + timeStamp + "." + extension);
            document.Close();
            misc.RemoveAdhoc();
        }


    }
}