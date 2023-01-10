using App1.libs;
using App1.Models;
using App1.Services;
using App1.Services.Engines;
using Syncfusion.ListView.XForms;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyDocuments : ContentPage
    {
        SfPopupLayout popupLayout;
        SfListView listView;
        List<Document> myDocs = new List<Document>();
        Misc misc = new Misc();
        public MyDocuments()
        {
            InitializeComponent();
            this.Appearing += LoadData;
            this.Init();
        }
        private async void Init()
        {
            try
            {               
                Title = "PDF Utility";
                //await Task.Delay(500);
                loader.IsEnabled = true;
                loader.IsVisible = true;
                    
                await Task.Run(() =>
                {
                // do work here that you don't want on the UI thread 
                /*if (Device.RuntimePlatform == Device.iOS)
                {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.Personal); 
                }
                else if (Device.RuntimePlatform == Device.Android)
                {
                    path = misc.GetPath();
                }*/
                    string path = misc.GetPath();
                    Documents docs = new Documents();
                    myDocs = docs.GetDocuments(path);                    
                    Device.BeginInvokeOnMainThread(() =>
                    {                       
                        if (myDocs.Count < 1)
                        {
                            img.Source = Device.RuntimePlatform == Device.Android
                            ? ImageSource.FromFile("error.png")
                            : ImageSource.FromFile("error.png");
                            SL.Children.Add(img);
                            TitleView.IsVisible = false;
                            loader.IsVisible = false;
                        }
                        else
                        {
                            TitleView.IsVisible = true;
                            SL.IsVisible = false;
                            loader.IsVisible = false;
                            MyListView.ItemsSource = myDocs;
                        }
                    });
                });
                popupLayout = new SfPopupLayout();
                popupLayout.PopupView.HeaderTitle = "Actions";
                MyListView.ItemTapped += Handle_ItemTapped;
                MyListView.SelectionGesture = Syncfusion.ListView.XForms.TouchGesture.Tap;
                MyListView.SelectionMode = Syncfusion.ListView.XForms.SelectionMode.Single;
                loader.IsEnabled = false;
                loader.IsVisible = false;
            }
            catch (Exception ex)
            {

            }





            /*BaseService bs = new BaseService();
            List<Config> configs=await bs.GetConfigs();
            foreach(Config c in configs)
            {
                if (c.name.Equals("ads") && c.value.Equals("1"))// admin has set to show ads on devices
                {
                    if (Device.RuntimePlatform == Device.Android)
                        adMobView.AdUnitId = "ca-app-pub-9511268744828643/1991257149";
                    else if (Device.RuntimePlatform == Device.iOS)
                        adMobView.AdUnitId = "ca-app-pub-9511268744828643/4042705410";

                }
            }*/
        }
        async void Handle_ItemTapped(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            try
            {
                var item = (Document)e.ItemData;
                popupLayout.PopupView.AnimationMode = AnimationMode.Zoom;
                popupLayout.PopupView.ContentTemplate = new DataTemplate(() =>
                {
                    return GetMenuList(item.name, item.path, item.img, item.path);
                });
                popupLayout.PopupView.ShowFooter = false;
                popupLayout.Show();
                MyListView.SelectedItem = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


      //  protected async override void OnAppearing()
        private async void LoadData(object sender, EventArgs e)
        {
            /*             this.Init();
                        loader.IsEnabled = false;
                        loader.IsVisible = false;*/

            try
            {
                Title = "PDF Utility";
                await Task.Delay(500);
                loader.IsEnabled = true;
                loader.IsVisible = true;

               //await Task.Run(async () =>
               // {
                    // do work here that you don't want on the UI thread 
                    /*if (Device.RuntimePlatform == Device.iOS)
                    {
                        path = Environment.GetFolderPath(Environment.SpecialFolder.Personal); 
                    }
                    else if (Device.RuntimePlatform == Device.Android)
                    {
                        path = misc.GetPath();
                    }*/
                    string path = misc.GetPath();
                    Documents docs = new Documents();
                    myDocs = docs.GetDocuments(path);
                  //  Device.BeginInvokeOnMainThread(() =>
                   // {
                        if (myDocs.Count < 1)
                        {
                            img.Source = Device.RuntimePlatform == Device.Android
                            ? ImageSource.FromFile("error.png")
                            : ImageSource.FromFile("error.png");
                            SL.Children.Add(img);
                            TitleView.IsVisible = false;
                            loader.IsVisible = false;
                        }
                        else
                        {
                            TitleView.IsVisible = true;
                            SL.IsVisible = false;
                            loader.IsVisible = false;
                            MyListView.ItemsSource = myDocs;
                        }
                 //   });
                //});
                popupLayout = new SfPopupLayout();
                popupLayout.PopupView.HeaderTitle = "Actions";
                MyListView.ItemTapped += Handle_ItemTapped;
                MyListView.SelectionGesture = Syncfusion.ListView.XForms.TouchGesture.Tap;
                MyListView.SelectionMode = Syncfusion.ListView.XForms.SelectionMode.Single;
                loader.IsEnabled = false;
                loader.IsVisible = false;
            }
            catch (Exception ex)
            {
                loader.IsEnabled = false;
                loader.IsVisible = false;
            }
            loader.IsEnabled = false;
            loader.IsVisible = false;

        }

        private SfListView GetMenuList(string name, string path, string imgSource, string package)
        {
            listView = new SfListView() { ItemSpacing = 5 };
            listView.WidthRequest = 350;
            listView.HeightRequest = 150;
            List<PopupMenu> menu = new List<PopupMenu>();
            menu.Add(new PopupMenu
            {
                imageSource = Xamarin.Forms.ImageSource.FromFile("share.png"),
                labelName = "Share " + name,
                path = path,
                Action = "share",
                package = package
            });
            /*menu.Add(new PopupMenu
            {
                imageSource = Xamarin.Forms.ImageSource.FromFile("save.png"),
                labelName = "Copy " + name + " to SD card.",
                path = path,
                Action = "copy",
                package = package
            });*/

            menu.Add(new PopupMenu
            {
                imageSource = Xamarin.Forms.ImageSource.FromFile("open.png"),
                labelName = "Open " + name,
                path = path,
                Action = "open",
                package = package
            });
            listView.ItemTemplate = new DataTemplate(() =>
            {
                ViewCell viewCell = new ViewCell();
                var grid = new Grid() { RowSpacing = 1 };
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 50 });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 200 });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 50 });
                var imageSource = new Image()
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    HeightRequest = 50
                };
                imageSource.SetBinding(Image.SourceProperty, new Binding("imageSource"));
                grid.Children.Add(imageSource, 0, 0);
                
                var labelName = new Label()
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    LineBreakMode = LineBreakMode.NoWrap,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    HeightRequest = 100,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = Font.SystemFontOfSize(NamedSize.Large).FontSize,
                };
                labelName.SetBinding(Label.TextProperty, new Binding("labelName"));
                grid.Children.Add(labelName, 1, 0);
                viewCell.View = grid;
                return viewCell;
            });


            listView.ItemsSource = menu;
            listView.ItemTapped += isOpenButton_Clicked;
            return listView;
        }
        private async void isOpenButton_Clicked(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            if (e == null || e.ItemData == null) return;
            var item = e.ItemData as PopupMenu;
            //            popupLayout.IsVisible = false;
            popupLayout.IsOpen = false;
            if (item.Action.Equals("share"))
            {
                try
                {
                    await Share.RequestAsync(new ShareFileRequest
                    {
                        File = new ShareFile(item.path),
                        Title = "Share file: " + item.labelName
                    });

                }
                catch (Exception ex)
                {
                    //misc.ShowToast("Failed to open the share dialog.");
                }

            }
            else if (item.Action.Equals("open"))
            {
                //Misc misc = new Misc();
                try
                {
                    //misc.OpenApp(item.package);
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(item.path)
                    });

                }
                catch (Exception ex)
                {
                    //misc.ShowToast("Failed to open that application.");
                }
            }
            else if (item.Action.Equals("copy"))
            {
                //Misc misc = new Misc();
                try
                {
                    FileInfo oFileInfo2 = new FileInfo(item.path);
                    string desFile = misc.GetPath() + "/" + oFileInfo2.Name;
                    File.Copy(item.path, desFile, true);
                    if (File.Exists(desFile))
                    {
                        //misc.ShowToast("File copied to: " + desFile);
                    }
                    else
                    {
                        //misc.ShowToast("Failed to copy file to: " + desFile);
                    }
                }
                catch (Exception ex)
                {
                    //misc.ShowToast("Failed to copy that file.");
                }
            }
        }

        private void SearchButton_Clicked(object sender, EventArgs e)
        {
            this.Search.IsVisible = true;
            this.AppTitle.IsVisible = false;
            this.SearchButton.IsVisible = false;          
            if (this.TitleView != null)
            {
                double opacity;

                // Animating Width of the search box, from 0 to full width when it added to the view.
                var expandAnimation = new Animation(
                    property =>
                    {
                        Search.WidthRequest = property;
                        opacity = property / TitleView.Width;
                        Search.Opacity = opacity;
                    }, 0, TitleView.Width, Easing.Linear);
                expandAnimation.Commit(Search, "Expand", 16, 250, Easing.Linear, (p, q) => this.SearchExpandAnimationCompleted());
            }

        }
        private void SearchExpandAnimationCompleted()
        {
            this.SearchEntry.Focus();
        }
        private void SearchEntry_Completed(object sender, EventArgs e)
        {
            var response = myDocs.Where(s => s.name.Contains(SearchEntry.Text));
            MyListView.ItemsSource = response;
        }

        private void closeSearch_Clicked(object sender, EventArgs e)
        {

        }
        private void inforSearch_Clicked(object sender, EventArgs e)
        {
            SfPopupLayout popupInfoLayout = new SfPopupLayout();
            popupInfoLayout.PopupView.HeaderTitle = "Afrodeb Studios";
            Label popupContent;
            //TextView popupContentView = new TextView(this) { Text = "AppShare was developed by Afrodeb Studios. \n For more information, visit afrodeb.com." };
            DataTemplate templateView = new DataTemplate(() =>
            {
                popupContent = new Label();
                popupContent.Text = "PDFUtility was developed by Afrodeb Studios. \n\n\nFor more information, visit afrodeb.com.";
                popupContent.FontSize = 16;
                popupContent.TextColor = Color.Black;
                popupContent.FontFamily = "{StaticResource Montserrat-Medium}";
                popupContent.Padding = new Thickness(10);
                //popupContent.BackgroundColor = Color.LightSkyBlue;
                //popupContent.HorizontalTextAlignment = TextAlignment.Center;
                return popupContent;
            });
            popupInfoLayout.PopupView.ContentTemplate = templateView;

            //popupInfoLayout.PopupView.ShowFooter = false;
            popupInfoLayout.PopupView.AnimationMode = AnimationMode.SlideOnTop;

            DataTemplate footerTemplateView = new DataTemplate(() =>
            {
                Button button = new Button();
                button.Text = "Go To Afrodeb.";
                button.Clicked += Afrodeb_Clicked;
                return button;
            });

            popupInfoLayout.PopupView.FooterTemplate = footerTemplateView;
            popupInfoLayout.Show();
        }

        private async void Afrodeb_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Browser.OpenAsync("https://afrodeb.com", BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                //misc.ShowToast("Failed to open the link. Please check if you have a browser installed.");
            }
        }
        private void BackToTitle_Clicked(object sender, EventArgs e)
        {
            this.SearchEntry.Text = ""; //clear the box
            this.Search.IsVisible = false;
            this.AppTitle.IsVisible = true;
            this.SearchButton.IsVisible = true;
            MyListView.ItemsSource = myDocs;
        }
    }
}