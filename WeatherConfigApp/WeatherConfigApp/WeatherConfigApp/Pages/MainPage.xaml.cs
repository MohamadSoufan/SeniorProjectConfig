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
            BluetoothLe = CrossBluetoothLE.Current;
            Adapter = CrossBluetoothLE.Current.Adapter;
            DeviceList = new ObservableCollection<IDevice>();
            Adapter.DeviceDiscovered += Adapter_DeviceDiscovered;

            //          Adapter.DeviceAdvertised += Adapter_DeviceAdvertised;
            DevicesListView.ItemsSource = DeviceList;
            DevicesListView.ItemTapped += DevicesListView_ItemTappedAsync;
  //          Adapter.DeviceConnected += Adapter_DeviceConnected;
            
        }

        private async void Adapter_DeviceConnected(object sender, DeviceEventArgs e)
        {
            await DisplayAlert("Connected", "yes " + BtDevice.Id, "ok");
        }

        private async void DevicesListView_ItemTappedAsync(object sender, ItemTappedEventArgs e)
        {

            try
            {
                if (Adapter.IsScanning)
                    await Adapter.StopScanningForDevicesAsync();
                BtDevice = DevicesListView.SelectedItem as IDevice;
                //await DisplayAlert("K",
                //    "typ: " + BtDevice.AdvertisementRecords[3].Type + "|msg: " + Encoding.UTF8.GetString(BtDevice.AdvertisementRecords[0].Data,0,1),
                //    "ok");


                // await Adapter.ConnectToDeviceAsync(BtDevice);
               
                var task = await Adapter.ConnectToKnownDeviceAsync(BtDevice.Id);
                

                //BtDevice = await Adapter.ConnectToKnownDeviceAsync(Guid.Parse("00000000-0000-0000-0000-B827EB7693D6"));


                // await Adapter.ConnectToDeviceAsync(BtDevice);

                //     await Navigation.PushAsync(new NetworkInfoPage(BluetoothLe,Adapter,BtDevice));
            }
            catch (DeviceConnectionException exception)
            {
                await DisplayAlert("Exception", exception.Message, "Exit");
            }

            // FatherStation.ConnectionStatus = "Connected";
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
        private void Adapter_DeviceDiscovered(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            DeviceList.Add(e.Device);
            Uuids.Add(e.Device.Id.ToString());
            //    foreach (var bytee in e.Device.AdvertisementRecords[1].Data)
            if(e.Device.AdvertisementRecords.Count>=2)
                Uuids.Add(e.Device.AdvertisementRecords[1].Data.ToString());
                
        }

        private async void ScanBtn_Clicked(object sender, EventArgs e)
        {
            UpdateUi(true, false);

            DeviceList.Clear();
            try
            {
                if (Adapter.IsScanning)
                    await Adapter.StopScanningForDevicesAsync();

                await BluetoothLe.Adapter.StartScanningForDevicesAsync();

            }
            catch (DeviceConnectionException exception)
            {
                UpdateUi(false, true);
                await DisplayAlert("Exception", exception.Message, "Exit");
            }
            // DevicesListView.ItemsSource = DeviceList.ToList();
            UpdateUi(false, true);
        }

        private async void SendBtn_Clicked(object sender, EventArgs e)
        {
            if (BtDevice != null)
            {
                try
                {
                    var services = await BtDevice.GetServicesAsync();

                    var isGuid = Guid.TryParse("12345678-1234-5678-1234-56789abc0100", out var guid);
                    var service =
                        await BtDevice.GetServiceAsync(guid);
                }
                catch (DeviceConnectionException exception)
                {
                    await DisplayAlert("Exception", exception.Message, "Exit");
                }
            }
            else
            {
                try
                {
                    var isGuid = Guid.TryParse("00000000-0000-0000-0000-b827eb7693d6",out var guid);


                    BtDevice = await Adapter.ConnectToKnownDeviceAsync(guid);

                }
                catch (DeviceConnectionException exception)
                {
                    await DisplayAlert("Exception", exception.Message, "Exit");                   
                }
            }
        }
        private void UpdateUi(bool wheel, bool btn)
        {
            SpinningWheel.IsRunning = wheel;
            SpinningWheel.IsVisible = wheel;
            ScanBtn.IsVisible = btn;
            ScanBtn.IsEnabled = btn;
        }
    }
}
