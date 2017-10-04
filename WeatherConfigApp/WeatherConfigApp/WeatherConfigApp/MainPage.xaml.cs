using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace WeatherConfigApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private async void BluetoothButton_Clicked(object sender, EventArgs e)
        {
            var boo = await DisplayAlert("haha", "Bluetooth it is", "Great!", "shit");

            if (boo)
                EntryField.Text = "Changed";
            else
                EntryField.Text = "Not Changed";

        }
        
    }
}
