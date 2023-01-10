using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App1.libs
{
    public class CustomEditor : Editor
    {
        public CustomEditor()
        {
        }

        public static readonly BindableProperty WatermarkTextProperty =
            BindableProperty.Create("WatermarkText", typeof(string), typeof(CustomEditor), "Enter a caption !!!", BindingMode.Default, null, null);


        public string WatermarkText
        {
            get { return (string)GetValue(WatermarkTextProperty); }
            set { SetValue(WatermarkTextProperty, value); }
        }
    }

    public class RoundedColorButton : Button
    {

    }


    public class CustomButton : Button
    {

    }
    public class CustomImageView : Image
    {


    }
}
