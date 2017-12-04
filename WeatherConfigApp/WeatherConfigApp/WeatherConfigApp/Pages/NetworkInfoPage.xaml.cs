using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherConfigApp.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NetworkInfoPage : ContentPage
	{
	    public static string NetworkSsid;
	    public static string NetworkPwd;
	    private IBluetoothLE BluetoothLe { get;}
        private IAdapter Adapter { get; }
	    private IDevice BtDevice { get; }
	    private IService Service { get; set; }

	    public NetworkInfoPage ()
		{
			InitializeComponent ();
		    NetworkPwdEntry.Text = "";
		    NetworkSsidEntry.Text = "";
		}

	    public NetworkInfoPage(IBluetoothLE bluetoothLe, IAdapter adapter, IDevice device):this()
	    {
	        BluetoothLe = bluetoothLe;
	        Adapter = adapter;
	        BtDevice = device;
	    }
        private async void SendBtn_Clicked(object sender, EventArgs e)
        {
            NetworkSsid = NetworkSsidEntry.Text;
            NetworkPwd = NetworkPwdEntry.Text;

            try
            {
                if (this.BtDevice == null)
                {
                    await DisplayAlert("Connecting Error", "Please try again", "OK");
                    return;
                }
                var services = await this.BtDevice.GetServicesAsync();
                // var service = await BtDevice.GetServiceAsync(Guid.Parse("12345678-1234-5678-1234-B827EB7693D6"));
                //  var chrs = await service.GetCharacteristicAsync(Guid.Parse("12345678-1234-5678-0000-B827EB7693D6"));
                if (services == null || services[0] == null) 
                {
                    await DisplayAlert("Connecting Error", "Please try again", "OK");
                    return;
                }
                var chrs = await services[0].GetCharacteristicsAsync();
                if (chrs == null || chrs[0] == null || chrs.Count<2)
                {
                    await DisplayAlert("Connecting Error", "Please try again", "OK");
                    return;
                }
                if (chrs[0].CanWrite)
                {
                    chrs[0].WriteType = CharacteristicWriteType.Default;
                    chrs[1].WriteType = CharacteristicWriteType.Default;

                    var ssidBytes = System.Text.Encoding.UTF8.GetBytes(NetworkSsid);
                    var pwdBytes = System.Text.Encoding.UTF8.GetBytes(NetworkPwd);
                    var byteslen = System.Text.Encoding.UTF8.GetByteCount(NetworkSsid);
                    var byteslen2 = System.Text.Encoding.UTF8.GetByteCount(NetworkPwd);


                    await chrs[0].WriteAsync(ssidBytes);
                    await chrs[1].WriteAsync(pwdBytes);
                }
            }
            catch (DeviceConnectionException exception)
            {
                try
                {
                    await this.Adapter.DisconnectDeviceAsync(this.BtDevice);
                }
                catch (DeviceConnectionException excep)
                {
                    await DisplayAlert("Exception", excep.Message, "Exit");
                }

                await DisplayAlert("Exception", exception.Message, "Exit");

            }
            //await Navigation.PopAsync();
        }

        private void NetworkSsidEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateUi();
        }

	    private void NetworkPwdEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateUi();
        }
	    private void UpdateUi()
	    {
	        if (NetworkPwdEntry.Text.Length > 0 && NetworkPwdEntry.Text.Length > 7)
	            SendBtn.IsEnabled = true;
	        else
	            SendBtn.IsEnabled = false;
	    }
    }
}