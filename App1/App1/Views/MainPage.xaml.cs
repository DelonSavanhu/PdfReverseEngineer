using App1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {
            try
            {
                InitializeComponent();

                MasterBehavior = MasterBehavior.Popover;

                MenuPages.Add((int)MenuItemType.Browse, (NavigationPage)Detail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task NavigateFromMenu(int id)
        {
            if (!MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.Browse:
                        MenuPages.Add(id, new NavigationPage(new ItemsPage()));
                        break;
                    case (int)MenuItemType.About:
                        MenuPages.Add(id, new NavigationPage(new AboutPage()));
                        break;
                    case (int)MenuItemType.PdfMerger:
                        MenuPages.Add(id, new NavigationPage(new PdfMerger()));
                        break;
                    case (int)MenuItemType.Documents:
                        MenuPages.Add(id, new NavigationPage(new Test()));
                        break;
                    case (int)MenuItemType.DocxToPdf:
                        MenuPages.Add(id, new NavigationPage(new DocXToPdf()));
                        break;
                    case (int)MenuItemType.HtmlToPdf:
                        MenuPages.Add(id, new NavigationPage(new HtmlToPdf()));
                        break;
                    case (int)MenuItemType.ImagesToPdf:
                        MenuPages.Add(id, new NavigationPage(new ImagesToPdf()));
                        break;
                    case (int)MenuItemType.PdfToDocx:
                        MenuPages.Add(id, new NavigationPage(new PdfToDocX()));
                        break;
                    case (int)MenuItemType.PdfToHtml:
                        MenuPages.Add(id, new NavigationPage(new PdfToHtml()));
                        break;
                    case (int)MenuItemType.PdfToImages:
                        MenuPages.Add(id, new NavigationPage(new PdfToImages()));
                        break;
                    case (int)MenuItemType.ScanToPdf:
                        MenuPages.Add(id, new NavigationPage(new ScanToPdf()));
                        break;
                    case (int)MenuItemType.ImageEdit:
                        MenuPages.Add(id, new NavigationPage(new ScanToPdf()));
                        break;
                }
            }

            var newPage = MenuPages[id];

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }
    }
}