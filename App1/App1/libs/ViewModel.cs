using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace App1.libs
{
    public class ViewModel
    {
        public ObservableCollection<Model> ModelList
        {
            get;
            set;
        }
        public ViewModel()
        {
            Assembly assembly = typeof(ImageSerializeModel).GetTypeInfo().Assembly;
            ModelList = new ObservableCollection<Model>
            {
                #if COMMONSB
                new Model { Name=ImageSource.FromResource("SampleBrowser.Icons.EditorDashboard.jpg",assembly),ImageName="Dashboard"} ,
                new Model { Name=ImageSource.FromResource("SampleBrowser.Icons.EditorSuccinity.png",assembly),ImageName="Succinity"} ,
                new Model { Name=ImageSource.FromResource("SampleBrowser.Icons.EditorTwitter.jpeg",assembly),ImageName="Twitter"} ,
                #else
                new Model { Name=ImageSource.FromResource("SampleBrowser.SfImageEditor.Icons.EditorDashboard.jpg",assembly),ImageName="Dashboard"} ,
                new Model { Name=ImageSource.FromResource("SampleBrowser.SfImageEditor.Icons.EditorSuccinity.png",assembly),ImageName="Succinity"} ,
                new Model { Name=ImageSource.FromResource("SampleBrowser.SfImageEditor.Icons.EditorTwitter.jpeg",assembly),ImageName="Twitter"} ,
                #endif
    };

        }
    }

    public class Model : INotifyPropertyChanged
    {
        private ImageSource name;
        public ImageSource Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged("Name");

            }
        }

        private string _imagestream;
        public string Imagestream
        {
            get { return _imagestream; }
            set
            {
                _imagestream = value;
                RaisePropertyChanged("Imagestream");
            }
        }

        private Stream _stream;
        public Stream Strm
        {
            get { return _stream; }
            set
            {
                _stream = value;
                RaisePropertyChanged("Strm");
            }
        }


        private string _imageName;
        public string ImageName
        {
            get { return _imageName; }
            set
            {
                _imageName = value;
                RaisePropertyChanged("ImageName");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }


}
