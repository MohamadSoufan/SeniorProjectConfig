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
    public partial class PairDevice : ContentPage
    {
        public IBluetoothLE BluetoothLe { get; set; }
        public IAdapter Adapter { get; set; }
        public IDevice BtDevice { get; set; }
        public ObservableCollection<IDevice> DeviceList { get; set; }
        public PairDevice()
        {
            InitializeComponent();
            BluetoothLe = CrossBluetoothLE.Current;
            Adapter = CrossBluetoothLE.Current.Adapter;
            DeviceList = new ObservableCollection<IDevice>();
            Adapter.DeviceDiscovered += Adapter_DeviceDiscovered;
            DevicesListView.ItemsSource = DeviceList;
            DevicesListView.ItemTapped += DevicesListView_ItemTapped;
           
        }

        private async void DevicesListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            BtDevice = e.Item as IDevice;
            try
            {
                await Adapter.ConnectToDeviceAsync(BtDevice);
            }
            catch (DeviceConnectionException exception)
            {

            }
        }

        private void Adapter_DeviceDiscovered(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            DeviceList.Add(e.Device);
        }

        private async void ScanBtn_Clicked(object sender, EventArgs e)
        {
            DeviceList.Clear();
            try
            {
                await BluetoothLe.Adapter.StartScanningForDevicesAsync();

            }
            catch (DeviceConnectionException exception)
            {
                await DisplayAlert("Exception", exception.Message, "Exit");
            }
            // DevicesListView.ItemsSource = DeviceList.ToList();
        }
    }
}