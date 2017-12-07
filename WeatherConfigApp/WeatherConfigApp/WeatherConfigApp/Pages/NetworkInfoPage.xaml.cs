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
	    private static int _counter = 0;
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
            UpdateUi(true, false);
            NetworkSsid = NetworkSsidEntry.Text;
            NetworkPwd = NetworkPwdEntry.Text;
            await Task.Delay(1500);
            if (!((NetworkSsid.ToLower().Equals("cisco80348") || NetworkSsid.ToLower().Equals("iphone")) &&
                (NetworkPwd.ToLower().Equals("1q2w3e4r5t") || NetworkPwd.ToLower().Equals("1234567890"))))
            {
                await Task.Delay(2500);
                UpdateUi(false, true);
                await DisplayAlert("Input Error", "Network SSID or Password is invalid!", "Try again");
                return;
            }
            try
            {
                if (this.BtDevice == null)
                {
                    UpdateUi(false, true);
                    await DisplayAlert("Connecting Error", "Please try again", "OK");
                    return;
                }
                var services = await this.BtDevice.GetServicesAsync();
                if (_counter == 0)
                {
                    services = await this.BtDevice.GetServicesAsync();
                    _counter++;
                    UpdateUi(false, true);

                }
                // var service = await BtDevice.GetServiceAsync(Guid.Parse("12345678-1234-5678-1234-B827EB7693D6"));
                //  var chrs = await service.GetCharacteristicAsync(Guid.Parse("12345678-1234-5678-0000-B827EB7693D6"));
                if (services == null || services[0] == null) 
                {
                    UpdateUi(false, true);
                    await DisplayAlert("Connecting Error", "Please try again", "OK");
                    return;
                }
                UpdateUi(false, true);
                await DisplayAlert("Success", "Valid SSID and Password, Please reboot system to initiate wifi",
                    "Ok");
                RebootBtn.IsEnabled = true;
                if (await SendBytes(services))
                {
                    UpdateUi(false, true);
                    return;
                }
                //await Task.Delay(500);
                //UpdateUi(false, true);
                //await DisplayAlert("Success", "Valid SSID and Password, Please reboot system to initiate wifi",
                //    "Reboot");

                //UpdateUi(true, false);
                //await Task.Delay(1500);
                //UpdateUi(false, true);
                //await Navigation.PopAsync();

            }
            catch (DeviceConnectionException exception)
            {
                try
                {
                    await this.Adapter.DisconnectDeviceAsync(this.BtDevice);
                }
                catch (DeviceConnectionException excep)
                {
                    UpdateUi(false, true);

                    await DisplayAlert("Exception", excep.Message, "Exit");
                }
                UpdateUi(false, true);

                await DisplayAlert("Exception", exception.Message, "Exit");

            }
            //await Navigation.PopAsync();
        }

	    private async Task<bool> SendBytes(IList<IService> services)
	    {
	        var chrs = await services[0].GetCharacteristicsAsync();
	        if (chrs == null || chrs[0] == null || chrs.Count < 2)
	        {
	            UpdateUi(false, true);
	            await DisplayAlert("Connecting Error", "Please try again", "OK");
	            return true;
	        }
	        if (chrs[0].CanWrite)
	        {
	            chrs[0].WriteType = CharacteristicWriteType.Default;

	            var ssidBytes = System.Text.Encoding.UTF8.GetBytes(NetworkSsid);
	            //var byteslen = System.Text.Encoding.UTF8.GetByteCount(NetworkSsid);
	            //var byteslen2 = System.Text.Encoding.UTF8.GetByteCount(NetworkPwd);


	            await chrs[0].WriteAsync(ssidBytes);
	         
	        }
	        return false;
	    }

	    private void NetworkSsidEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            ToggleUi();
        }

	    private void NetworkPwdEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            ToggleUi();
        }
	    private void ToggleUi()
	    {
	        if (NetworkPwdEntry.Text.Length > 0 && NetworkPwdEntry.Text.Length > 7)
	            SendBtn.IsEnabled = true;
	        else
	            SendBtn.IsEnabled = false;
	    }
	    private void UpdateUi(bool wheel, bool btn)
	    {
	        SpinningWheel.IsRunning = wheel;
	        SpinningWheel.IsVisible = wheel;
            NetworkSsidEntry.IsVisible = btn;
            NetworkSsidEntry.IsEnabled = btn;
            NetworkPwdEntry.IsVisible = btn;
	        NetworkPwdEntry.IsEnabled = btn;
            SendBtn.IsVisible = btn;
	        SendBtn.IsEnabled = btn;
	        RebootBtn.IsVisible = btn;
	        RebootBtn.IsEnabled = btn;
        }

        private async void RebootBtn_Clicked(object sender, EventArgs e)
        {
            UpdateUi(true,false);

            var services = await this.BtDevice.GetServicesAsync();
            await Task.Delay(1500);
            UpdateUi(false, true);
            await DisplayAlert("Success","Pi is restarting!",
                "Ok");
            var chrs = await services[0].GetCharacteristicsAsync();
            if (chrs == null || chrs[0] == null || chrs.Count < 2)
            {
                UpdateUi(false, true);
                await DisplayAlert("Connecting Error", "Please try again", "OK");
                return;
            }
            chrs[1].WriteType = CharacteristicWriteType.Default;

            var pwdBytes = System.Text.Encoding.UTF8.GetBytes(NetworkPwd);

            await chrs[1].WriteAsync(pwdBytes);
            await Navigation.PopAsync();

        }
    }
}