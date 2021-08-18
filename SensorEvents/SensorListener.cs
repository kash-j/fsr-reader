using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorEvents
{
    public interface SensorListener
    {
        /// <summary>
        /// Output - Output method which retrieves all the sensor data
        /// </summary>
        /// <param name="data"></param>
        void Output(SensorData data);
    }
}
