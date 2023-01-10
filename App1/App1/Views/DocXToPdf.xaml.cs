using App1.libs;
using App1.Models;
using App1.Services;
using App1.Services.Engines;
using App1.Services.Interfaces;
using Newtonsoft.Json;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Presentation;
using Syncfusion.PresentationToPdfConverter;
using Syncfusion.XlsIO;
using Syncfusion.XlsIORenderer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace App1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocXToPdf : ContentPage
    {
        IDownloader downloader = DependencyService.Get<IDownloader>();
        Misc misc = new Misc();

        public ObservableCollection<string> Items { get; set; }
        public List<string> request = new List<string>();
        ObservableCollection<Document> request2 { get; set; }
        public DocXToPdf()
        {
            InitializeComponent();
            Title = "Office files to PDF Conversion.";
            Items = new ObservableCollection<string>();
            //request2 = new ObservableCollection<Document>();
            //MyListView.ItemsSource = request2;
            //MyListView.ItemTapped += Handle_ItemTapped;

            this.Init();

        }

        private async void Init()
        {
            loader.IsRunning = true;
            loader.IsVisible = false;
            request2 = new ObservableCollection<Document>();
            MyListView.ItemsSource = request2;
            MyListView.ItemTapped += Handle_ItemTapped;
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
                request2.Remove(selectedItem);
                request.RemoveAt(index);// remoce from list of items to be sent to the server
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
                if (!misc.AllowedTypes(Path.GetExtension(filePath), new string[] { ".docx", ".ppt", ".pptx", ".xlsx", ".doc" }))
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


                if (Items != null && Items.Count > 0)
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
                int counter = 0;
                await Task.Delay(500);
                for (int x=0; x< request.Count; x++)
                {
                    if ((Path.GetExtension(request[x]).Equals(".docx") || Path.GetExtension(request[x]).Equals(".doc")) && this.WordDocuments(request[x]))
                    {
                        counter++;
                    }
                    else if((Path.GetExtension(request[x]).Equals(".xlsx") || Path.GetExtension(request[x]).Equals(".xls")) && this.XlsDocuments(request[x]))
                    {
                        counter++;
                    }else if ((Path.GetExtension(request[x]).Equals(".ppt") || Path.GetExtension(request[x]).Equals(".pptx")) && this.PPTDocuments(request[x]))
                    {
                        counter++;
                    }
                    else
                    {
                        throw new Exception("failed to genderate files.");
                    }
                }
                if (counter == request.Count)
                {
                    await DisplayAlert("All Done!", "Your files have been generated.", "Ok");
                    this.Init(); //reset everything
                    await Navigation.PushAsync(new MyDocuments());
                }
                else if(counter > 0 && counter < request.Count)
                {
                    await DisplayAlert("All Done!", "Some files were generated successfuly.", "Ok");
                    this.Init(); //reset everything
                    await Navigation.PushAsync(new MyDocuments());
                }
                else
                {
                    throw new Exception("Could not generate files");
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

        protected override void OnAppearing()
        {
            //   DisplayAlert("test", "OnResume", "Ok");
        }
        private bool XlsDocuments(string path)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                //Gets input Excel document from an embedded resource collection
                Stream excelStream = File.OpenRead(path); //assembly.GetManifestResourceStream("ExcelToPDF.xlsx");
                IWorkbook workbook = application.Workbooks.Open(excelStream);
                //Initialize XlsIO renderer.
                XlsIORenderer renderer = new XlsIORenderer();
                //Convert Excel document into PDF document 
                PdfDocument pdfDocument = renderer.ConvertToPDF(workbook);
                //Save the PDF document to stream.
                MemoryStream streamed = new MemoryStream();
                pdfDocument.Save(streamed);
                streamed.Position = 0;
                excelStream.Position = 0;
                string timeStamp = DateTime.Now.ToFileTime().ToString();
                misc.RemoveAdhoc();
                return misc.CopyStream(streamed, misc.GetPath() + "/merged_" + timeStamp + ".pdf");
            }
        }
        private bool WordDocuments(string path)
          {
            //Loads the file as stream
                            Stream stream1 = File.OpenRead(path);
                        //Load the stream into word document
                        WordDocument wordDocument = new WordDocument(stream1, Syncfusion.DocIO.FormatType.Automatic);

                            DocIORenderer render = new DocIORenderer();
                            //Converts Word document into PDF document
                            PdfDocument pdfDocument = render.ConvertToPDF(wordDocument);
                            //Releases all resources used by the Word document and DocIO Renderer objects
                            render.Dispose();
                            wordDocument.Dispose();
                            //Save the document into memory stream
                            MemoryStream stream = new MemoryStream();
                            pdfDocument.Save(stream);
                            stream.Position = 0;
                            //Close the document 
                            pdfDocument.Close();
                        string timeStamp = DateTime.Now.ToFileTime().ToString();
            misc.RemoveAdhoc();
            return misc.CopyStream(stream, misc.GetPath() + "/merged_" + timeStamp + ".pdf");
            }

        private bool PPTDocuments(string path)
        {
            //Loads the file as stream
            Stream stream1 = File.OpenRead(path);
            using (IPresentation pptxDoc = Presentation.Open(stream1))
            {
                //Create the MemoryStream to save the converted PDF.
                using (MemoryStream pdfStream = new MemoryStream())
                {
                    //Convert the PowerPoint document to PDF document.
                    using (PdfDocument pdfDocument = PresentationToPdfConverter.Convert(pptxDoc))
                    {
                        //Save the converted PDF document to MemoryStream.
                        pdfDocument.Save(pdfStream);
                        string timeStamp = DateTime.Now.ToFileTime().ToString();
                        //pdfStream.Position = 0;
                        //stream1.Position = 0;
                        misc.RemoveAdhoc();
                        return misc.CopyStream(pdfStream, misc.GetPath() + "/merged_" + timeStamp + ".pdf");
                    }
                    //Create the output PDF file stream

                }
            }
        }

       


    }
}