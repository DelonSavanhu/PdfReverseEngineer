//using SampleBrowser.Core;
using SampleBrowser.Core;
using Syncfusion.SfImageEditor.XForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using App1.libs;

namespace App1.Views
{
    public partial class ImageEdit : SampleView
    {
        double height = 0, width = 0;

        internal static bool isLoaded = false;


        void ImageTapped(object sender, System.EventArgs e)
        {
            LoadFromStream((sender as Image).Source);
        }

        void LoadFromStream(ImageSource source)
        {
            if (Device.RuntimePlatform.ToLower() == "ios")
            {
                Navigation.PushAsync(new NavigationPage(new SfImageEditorPage(source)));
            }
            else if (Device.RuntimePlatform.ToLower() == "uwp")
            {
                Navigation.PushAsync(new SfImageEditorPage(source));
            }
            else
            {
                Navigation.PushModalAsync(new SfImageEditorPage(source));
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (Device.RuntimePlatform.ToLower() == "uwp" && Device.Idiom == TargetIdiom.Desktop)
            {
                imageGrid.RowDefinitions.Clear();
                imageGrid.Padding = new Thickness(20, 70, 20, 0);
                imageGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.1, GridUnitType.Star) });
                imageGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2, GridUnitType.Star) });
                imageGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.1, GridUnitType.Star) });
                mainGrid.ColumnDefinitions.Clear();
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }
            else if ((width != this.width || height != this.height) && (width > -1 || height > -1))
            {
                this.width = width;
                this.height = height;
                if (width < height)
                {
                    imageGrid.RowDefinitions.Clear();
                    imageGrid.Padding = new Thickness(20, 70, 20, 0);
                    imageGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.1, GridUnitType.Star) });
                    imageGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.6, GridUnitType.Star) });
                    imageGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    mainGrid.ColumnDefinitions.Clear();
                    mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                }
                else
                {
                    imageGrid.RowDefinitions.Clear();
                    mainGrid.ColumnDefinitions.Clear();
                    imageGrid.Padding = new Thickness(20, 10, 20, 0);
                    imageGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.1, GridUnitType.Star) });
                    imageGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1.6, GridUnitType.Star) });
                    imageGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Star) });
                    mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.5, GridUnitType.Star) });
                    mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.5, GridUnitType.Star) });
                    mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.5, GridUnitType.Star) });
                }
            }
        }
        ImageModel model;
        public ImageEdit()
        {
            model = new ImageModel();
            BindingContext = model;
            InitializeComponent();
        }
    }

    public class ImageModel : INotifyPropertyChanged
    {
        public ImageSource BroweImage1 { get; set; }
        public ImageSource BroweImage2 { get; set; }
        public ImageSource BroweImage3 { get; set; }
        public int UndoCount { get; set; }
        public int RedoCount { get; set; }

        private bool isColorPaletteVisible;

        public bool IsColorPaletteVisible
        {
            get { return isColorPaletteVisible; }
            set
            {
                isColorPaletteVisible = value;
                OnPropertyChanged("IsColorPaletteVisible");
            }
        }

        public bool IsImageEdited { get; set; }

        private bool isTouched;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsTouched
        {
            get { return isTouched; }
            set
            {
                isTouched = value;
                OnPropertyChanged("IsTouched");
            }
        }

        public ImageModel()
        {
            Assembly assembly = typeof(ImageSerializeModel).GetTypeInfo().Assembly;
#if COMMONSB
			BroweImage1 = ImageSource.FromResource("http://172.105.181.118/appdata/word.png",assembly);
			BroweImage2 = ImageSource.FromResource("http://172.105.181.118/appdata/word.png",assembly);
			//BroweImage3 = ImageSource.FromResource("http://172.105.181.118/appdata/word.png",assembly);
            BroweImage3 = ImageSource.FromUri(new Uri("http://172.105.181.118/appdata/word.png")/*, assembly*/);
#else
            BroweImage1 = ImageSource.FromResource("http://172.105.181.118/appdata/word.png", assembly);
            BroweImage2 = ImageSource.FromResource("http://172.105.181.118/appdata/word.png", assembly);
            BroweImage3 = ImageSource.FromUri(new Uri("http://172.105.181.118/appdata/word.png")/*, assembly*/);
           
#endif
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void DetectTouch()
        {
            IsTouched = !isTouched;
            OnPropertyChanged("IsTouched");
        }
    }

    public class SfImageEditorPage : ContentPage
    {
        public SfImageEditorPage(ImageSource imagesource)
        {
            Syncfusion.SfImageEditor.XForms.SfImageEditor editor = new Syncfusion.SfImageEditor.XForms.SfImageEditor();
            editor.Source = imagesource;
            editor.RotatableElements = ImageEditorElements.Text;
            Content = editor;
        }
    }

}