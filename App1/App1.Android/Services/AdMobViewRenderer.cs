﻿using System;
using System.ComponentModel;
using Android.Content;
//using Android.Gms.Ads;
using Android.Widget;
using App1.Droid.Services;
using App1.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


//[assembly: ExportRenderer(typeof(AdMobView), typeof(AdMobViewRenderer))]
namespace App1.Droid.Services
{
/*    public class AdMobViewRenderer : ViewRenderer<AdMobView, AdView>
    {
        public AdMobViewRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<AdMobView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null && Control == null)
            {
                SetNativeControl(CreateAdView());
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                base.OnElementPropertyChanged(sender, e);

                if (e.PropertyName == nameof(AdView.AdUnitId))
                {
                    if (string.IsNullOrEmpty(Control.AdUnitId))
                    {
                        Control.AdUnitId = Element.AdUnitId;
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private AdView CreateAdView()
        {
            var adView = new AdView(Context)
            {
                AdSize = AdSize.SmartBanner,
                AdUnitId = Element.AdUnitId
            };

            adView.LayoutParameters = new LinearLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);

            adView.LoadAd(new AdRequest.Builder().Build());

            return adView;
        }

    }*/
}