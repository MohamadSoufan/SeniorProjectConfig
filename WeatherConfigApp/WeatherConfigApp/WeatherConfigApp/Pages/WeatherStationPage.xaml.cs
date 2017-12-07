using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherConfigApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WeatherStationPage : ContentPage
    {
        public WeatherStation Station { get; set; }

        public WeatherStationPage()
        {
            InitializeComponent();
        }

        public WeatherStationPage(WeatherStation station):this()
        {
            
            Station = station;
            Title = station.Name;
            NameLabel.Text = station.Name;
            StatusLabel.Text = station.ConnectionStatus;
        }
    

        private void ConnectBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ConnectDevice(Station));
        }
    }
}