using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace App1.libs
{
    public class ImageSerializeModel
    {
        public ImageSource BroweImage1 { get; set; }
        public ImageSource BroweImage2 { get; set; }
        public ImageSource BroweImage3 { get; set; }

        public ImageSerializeModel(ViewModel viewmodel)
        {
            BroweImage1 = viewmodel.ModelList[0].Name;
            BroweImage2 = viewmodel.ModelList[1].Name;
            BroweImage3 = viewmodel.ModelList[2].Name;
        }
    }
}
