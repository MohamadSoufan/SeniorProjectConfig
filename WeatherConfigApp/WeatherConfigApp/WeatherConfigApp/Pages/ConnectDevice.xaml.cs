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
            DevicesListView.ItemsSource = DeviceList;
            DevicesListView.ItemTapped += DevicesListView_ItemTapped;

           
        }
        public ConnectDevice(WeatherStation weatherstation):this()
        {
            FatherStation = weatherstation;
        }

        private async void DevicesListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            
            BtDevice = e.Item as IDevice;
            
            try
            {
                if(Adapter.IsScanning)
                    await Adapter.StopScanningForDevicesAsync();
                
                await Adapter.ConnectToDeviceAsync(BtDevice);

            }
            catch (DeviceConnectionException exception)
            {
                await DisplayAlert("Exception", exception.Message, "Exit");
                return;
            }
           
            FatherStation.ConnectionStatus = "Connected";
        }

        private void Adapter_DeviceDiscovered(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
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
    }
}