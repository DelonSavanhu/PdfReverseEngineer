using App1.Models;
using App1.Services;
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
    public partial class Ads : ContentView
    {
        public Ads()
        {
            InitializeComponent();
            Init();
        }
        public async void Init()
        {
            try
            {
                BaseService bs = new BaseService();
                List<Config> configs = await bs.GetConfigs();
                foreach (Config c in configs)
                {
                    if (c.name.Equals("ads") && c.value.Equals("1"))// admin has set to show ads on devices
                    {
                        if (Device.RuntimePlatform == Device.Android)
                        {
                            if (string.IsNullOrEmpty(adMobView.AdUnitId))
                            {
                                adMobView.AdUnitId = "ca-app-pub-9511268744828643/6931181492";
                            }
                        }
                        else if (Device.RuntimePlatform == Device.iOS)
                        {
                            adMobView.AdUnitId = "ca-app-pub-9511268744828643/4042705410";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
            
    }
}