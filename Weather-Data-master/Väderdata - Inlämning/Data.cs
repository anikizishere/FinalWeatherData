using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Väderdata___Inlämning
{
    internal class Data
    {
        public DateTime DateTime { get; set; }
        public float Temperature { get; set; }
        public int Humidity { get; set; }
        public Data(DateTime DateTime, float Temperature, int Humidty)
        {
            this.DateTime = DateTime;
            this.Temperature = Temperature;
            this.Humidity = Humidty;
        }
    }
}
