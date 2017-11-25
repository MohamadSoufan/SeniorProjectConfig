using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherConfigApp.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherConfigApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
   
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            using (var dbConnection = new SQLite.SQLiteConnection(App.DbPath))
            {
                
                dbConnection.CreateTable<WeatherStation>();
                var stations = dbConnection.Table<WeatherStation>().ToList();
                StationsListView.ItemsSource = stations;
            }
        }

        private async void BluetoothButton_Clicked(object sender, EventArgs e)
        {
            var boo = await DisplayAlert("haha", "Bluetooth it is", "Great!", "Cancel");
            
            //if (NetworkSsidEntry != null) NetworkSsidEntry.Text = boo ? "Changed" : "Not Changed";
        }

        private void ToolbarItem_Activated(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddWeatherStationPage());
        }

        private void BluetoothChangeStatusButton_Clicked(object sender, EventArgs e)
        {
          //  BluetoothStatusLabel.Text = BluetoothLe.State.ToString();

        }

        private void BluetoothSwitchCell_OnChanged(object sender, ToggledEventArgs e)
        {
            
        }

        private void StationsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Navigation.PushAsync(new WeatherStationPage(e.Item as WeatherStation));
        }
    }
}
