using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace App1.Themes
{
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DarkTheme:ResourceDictionary
    {
        public DarkTheme()
        {
            InitializeComponent();
        }
    }

}