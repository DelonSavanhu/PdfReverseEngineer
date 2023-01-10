using App1.libs;
using App1.Models;
using App1.Services.Engines;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Test : ContentPage
    {
        public Test()
        {
            Misc misc = new Misc();
            try {
                InitializeComponent();

                string path = "";
                if (Device.RuntimePlatform == Device.iOS)
                {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                }
                else if (Device.RuntimePlatform == Device.Android)
                {
                    path = misc.GetPath();// System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                }

                Documents docs = new Documents();
                List<Document> myDocs = docs.GetDocuments(path);
                DocumentsList.ItemsSource = myDocs;

                /*listView.ItemTemplate = new DataTemplate(() => {
                    var grid = new Grid();
                    var bookName = new Label { FontAttributes = FontAttributes.Bold, BackgroundColor = Color.Teal, FontSize = 21 };
                    bookName.SetBinding(Label.TextProperty, new Binding("BookName"));
                    var bookDescription = new Label { BackgroundColor = Color.Teal, FontSize = 15 };
                    bookDescription.SetBinding(Label.TextProperty, new Binding("BookDescription"));

                    grid.Children.Add(bookName);
                    grid.Children.Add(bookDescription, 1, 0);

                    return grid;
                });*/
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            }
    }
}