using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using SQLite.Net.Interop;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherConfigApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddWeatherStationPage : ContentPage
	{
		public AddWeatherStationPage ()
		{
			InitializeComponent ();
		}

        private void BtPairButton_Clicked(object sender, EventArgs e)
        {

        }

        private void AddButton_Clicked(object sender, EventArgs e)
        {
            var station = new WeatherStation
            {
                Name = StationNameEntry.Text ?? "Empty Station"
            };
            using (var dbConnection = new SQLite.SQLiteConnection( App.DB_PATH))
            {
                dbConnection.CreateTable<WeatherStation>();
                var numOfRows = dbConnection.Insert(station);
                if (numOfRows > 0)
                    DisplayAlert("Success", "Weather Station (" + station.Name + ") was created!", "Great!");
                else
                    DisplayAlert("Failure", "Weather Station (" + station.Name + ") failed to insert", "Terminate");
            }
        }
    }
}