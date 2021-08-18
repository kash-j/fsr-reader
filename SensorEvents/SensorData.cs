using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorEvents 
{
    public class SensorData 
    {
        /// <summary>
        /// Name - Name of the sensor
        /// Pressure - Amount of incoming pressure
        /// </summary>
        public string Name { get; set; }
        public int Pressure { get; set; }

        public SensorData(string name, int pressure)
        {
            this.Name = name;
            this.Pressure = pressure;
        }
    }
}
