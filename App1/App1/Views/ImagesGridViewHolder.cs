using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace App1.Views
{
    // make your custom cell
    public class ImagesGridViewHolder
    {
        public Image img;
        public Label lbl;
        public ImageTextCombo itc;

        public ImagesGridViewHolder()
        {
            img = new Image();
            lbl = new Label();
            itc = new ImageTextCombo();
            img.Aspect = Aspect.Fill;
        }

        // to get the root view of the your cell
        public View getRootView()
        {
            return img;
        }
    }
    public class ImageTextCombo
    {
        public Image img;
        public Label lbl;
    }

}
