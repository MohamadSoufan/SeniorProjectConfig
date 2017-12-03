using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherConfigApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectDevice : ContentPage
    {
        public string UuidString { get; set; }
        public IBluetoothLE BluetoothLe { get; set; }
        public IAdapter Adapter { get; set; }
        public IDevice BtDevice { get; set; }
        public ObservableCollection<IDevice> DeviceList { get; set; }
        public WeatherStation FatherStation { get; set; }
        public ConnectDevice()
        {
            InitializeComponent();
            BluetoothLe = CrossBluetoothLE.Current;
            Adapter = CrossBluetoothLE.Current.Adapter;
            DeviceList = new ObservableCollection<IDevice>();
            Adapter.DeviceDiscovered += Adapter_DeviceDiscovered;
  //          Adapter.DeviceAdvertised += Adapter_DeviceAdvertised;
            DevicesListView.ItemsSource = DeviceList;
            DevicesListView.ItemTapped += DevicesListView_ItemTapped;
            Adapter.DeviceConnected += Adapter_DeviceConnected;
            

           
        }

        private async void Adapter_DeviceConnected(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            await DisplayAlert("Connected", "yes " + UuidString, "ok");
           
        }

        private async void Adapter_DeviceAdvertised(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            try
            {
                await DisplayAlert("Alert",
                    "id= " + e.Device.Id+ " ARtype: " +
                    e.Device.AdvertisementRecords[0].Type, "ok");
            }
            catch (Exception exe)
            {
                await DisplayAlert("Exception",exe.Message,"cancel");
            }
        }

        public ConnectDevice(WeatherStation weatherstation):this()
        {
            FatherStation = weatherstation;
        }

        private async void DevicesListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {

            
            try
            {
                if(Adapter.IsScanning)
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

        private void Adapter_DeviceDiscovered(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            UuidString = e.Device.Id.ToString();
            DeviceList.Add(e.Device);
            
        }

        private async void ScanBtn_Clicked(object sender, EventArgs e)
        {
            UpdateUi(true,false);

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

        private void UpdateUi(bool wheel, bool btn )
        {
            SpinningWheel.IsRunning = wheel;
            SpinningWheel.IsVisible = wheel;
            ScanBtn.IsVisible = btn;
            ScanBtn.IsEnabled = btn;
        }

        private async void SendBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                var services = await BtDevice.GetServicesAsync();

                var service =
                    await BtDevice.GetServiceAsync(new Guid());
            }
            catch (DeviceConnectionException exception)
            {
                await DisplayAlert("Exception", exception.Message, "Exit");
            }
        }
    }
}