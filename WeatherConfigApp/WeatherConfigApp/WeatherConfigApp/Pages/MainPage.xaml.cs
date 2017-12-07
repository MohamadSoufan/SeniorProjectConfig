using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.Comgo.Ble;
using Plugin.Comgo.Ble.Abstractions;

using WeatherConfigApp.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherConfigApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public IBluetoothLE BluetoothLe { get; set; }
        public IAdapter Adapter { get; set; }
        public IDevice BtDevice { get; set; }
        public ObservableCollection<IDevice> DeviceList { get; set; }
        public List<string > Uuids { get; set; }
        public MainPage()
        {

            Uuids = new List<string>();
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
            var station = e.Item as WeatherStation;
            if (StationsListView.SelectedItem != null) 
            StationsListView.SelectedItem = null;
            Navigation.PushAsync(new WeatherStationPage(station));
        }
     

   
        
       
    }
}
