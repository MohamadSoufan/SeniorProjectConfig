using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using SQLite.Net.Interop;
using WeatherConfigApp.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherConfigApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddWeatherStationPage : ContentPage
	{

        protected WeatherStation Station;
        public AddWeatherStationPage ()
		{
			InitializeComponent ();
            AddButton.IsEnabled = false;
            
        }
     
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Station == null) return;
            if (Station.ConnectionStatus.Equals("Connected"))
                AddButton.IsEnabled = true;
        }
        private async void BtConnectButton_Clicked(object sender, EventArgs e)
	    {
	        if (string.IsNullOrEmpty(StationNameEntry.Text))
	        {
	            await DisplayAlert("Alert", "Station name can't be empty", "Cancel");
	            return;
	        }
            Station = new WeatherStation
	        {
	            Name = StationNameEntry.Text,
	            ConnectionStatus = "Disconnected"
	        };
            await Navigation.PushAsync(new ConnectDevice(Station));
	    }

        

	    private async void AddButton_Clicked(object sender, EventArgs e)
        {
       
                    
            using (var dbConnection = new SQLite.SQLiteConnection( App.DbPath))
            {
                dbConnection.CreateTable<WeatherStation>();
                
                var numOfRows = dbConnection.Insert(Station);
                if (numOfRows > 0)
                    await DisplayAlert("Success", "Weather Station (" + Station.Name + ") was created!", "Great!");
                else
                    await DisplayAlert("Failure", "Weather Station (" + Station.Name + ") failed to insert", "Terminate");
            }
        }
    }
}