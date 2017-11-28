using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
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
                var services = await BtDevice.GetServicesAsync();
            }
            catch (Exception exception)
            {
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