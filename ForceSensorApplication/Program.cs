using SensorEvents;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace ForceSensorApplication {
    class Program
    {
        static void Main(string[] args)
        {
            // Test class for testing of the sensorevents..
            TestClass test = new TestClass();
        }
    }

    public class TestClass : SensorListener
    {
        public TestClass()
        {
            SensorManager.AddListener(this);
            SensorManager.Threshold = 700;
            SensorManager.Start();
        }

        public void Output(SensorData data)
        {
            Console.WriteLine($"{data.Name} - {data.Pressure}");
        }
    }
}
