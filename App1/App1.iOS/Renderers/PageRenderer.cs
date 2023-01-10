using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App1.Themes;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ContentPage), typeof(App1.iOS.Renderers.PageRenderer))]
namespace App1.iOS.Renderers
{
    public class PageRenderer : Xamarin.Forms.Platform.iOS.PageRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;


            try
            {
                SetTheme();
            }
            catch (Exception ex) { }
        }
        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);

            if (TraitCollection.UserInterfaceStyle != previousTraitCollection.UserInterfaceStyle)
                SetTheme();
        }
        private void SetTheme()
        {
            if (TraitCollection.UserInterfaceStyle == UIUserInterfaceStyle.Dark)
                App.Current.Resources = new DarkTheme(); // needs using DarkMode.Styles;
            else
                App.Current.Resources = new LightTheme();
        }
    }
}
