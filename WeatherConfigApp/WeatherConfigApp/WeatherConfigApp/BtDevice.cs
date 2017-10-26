using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherConfigApp
{
    public class BtDevice
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public Guid Guid { get; set; }
        public string NetworkSsid { get; set; }
        public string NetworkPwk { get; set; }
    }
}
