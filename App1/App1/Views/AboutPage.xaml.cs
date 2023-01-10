using App1.Models;
using App1.Services.Engines;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        ArrayList request = new ArrayList();
        public AboutPage()
        {
            InitializeComponent();
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
                Console.WriteLine(filePath);
                lbl.Text = name;

                request.Add(filePath);
                DocX dx = new DocX();
                DocX_Response response = await dx.UploadDocx(new Models.DocX_Request
                {
                    filename = request
                });
                //Console.WriteLine("server response: "+response.response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}