using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using App1.Views;
using Syncfusion.ListView.XForms;
using App1.Models;
using App1.Services;
using Plugin.Toasts;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace App1
{
    public partial class App : Application
    {
        SfListView listView;
        public App()
        {
            try
            {
                InitializeComponent();
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzYzNjIxQDMxMzgyZTMzMmUzME9FN1BTU0FETHlyMnpacG1VclBTZzRGby9tNUszLzI5QlBLa2lMSjdvZkk9");
                AppCenter.LogLevel = LogLevel.Verbose;
                AppCenter.Start("ios=b01cdaf4-e46f-45af-b544-e4e76d35ebe1;android=ba539e3c-1119-41bd-bb1b-a7801e74deeb", typeof(Analytics), typeof(Crashes));                
                Crashes.NotifyUserConfirmation(UserConfirmation.AlwaysSend);
                MainPage = new MainPage();
                bool agree = false;
                Crashes.ShouldAwaitUserConfirmation = () =>
                {
                   MainPage.DisplayAlert("Error Reporting", "We would like to send errors to our developers. Do you agree?", "Agree", "Disagree");
                    //if (agree)
                    {

                    }
                    return true;
                };
                //Crashes.GenerateTestCrash();

                Crashes.SentErrorReport += (sender, e) =>
                {
                    MainPage.DisplayAlert("Error", "An error occured and we have submitted it for review.", "ok");
                };
                //MainPage =new NavigationPage(new MainPage());
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
        



    }
}
