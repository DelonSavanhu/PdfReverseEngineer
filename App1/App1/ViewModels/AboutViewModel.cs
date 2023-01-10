using App1.Services.Engines;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace App1.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";

            try
            {
                // Use whatever folder path you want here, the special folder is just an example
               
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }


            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
        }

        public ICommand OpenWebCommand { get; }
    }
}