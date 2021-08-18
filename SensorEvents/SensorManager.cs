using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SensorEvents
{
    public static class SensorManager
    {
        /// <summary>
        /// Port used for the arduino
        /// </summary>
        public static string PortName { get; set; }
        /// <summary>
        /// Serial Baudrate of the arduino
        /// </summary>
        public static int BaudRate { get; set; }
        /// <summary>
        ///  Wait time before checking next pressure hit
        /// </summary>
        public static int Wait { get; set; }
        /// <summary>
        /// Threshold before recognizing a pressure hit
        /// </summary>
        public static int Threshold { get; set; }
        /// <summary>
        /// Only the manager is permitted to determen his own Running Status
        /// </summary>
        public static bool IsRunning { get; private set; }

        // List of all listiner objects
        private static List<SensorListener> _listeners = new List<SensorListener>();
        // Serial port the manager is listening on
        private static SerialPort _serialPort = new SerialPort();
        // Local instance for the listenThread
        private static Thread _listenThread;
        // Is pressed status (this it to prevent constant hit registration)
        private static bool _isPressed;

        /// <summary>
        /// Static Contructor, Sets defaults for property's
        /// </summary>
        static SensorManager()
        {
            SensorManager.PortName = "COM3";
            SensorManager.BaudRate = 9600;
            SensorManager.Wait = 10;
            SensorManager.Threshold = 1010;
            SensorManager.IsRunning = false;

            // Default port setting (Port needs to be configured before Start Function) 
            SensorManager._serialPort.PortName = PortName;
            SensorManager._serialPort.BaudRate = BaudRate;
        }

        /// <summary>
        /// Sensor start event, sets the latest property data for the serial port
        /// when set then open new thread an start listeing on that port
        /// </summary>
        public static void Start()
        {
            // If the Serial port is open than start SerialReader
            // If Open() sucseeds the .IsOpen will be true
            // Before starting check if port is availible
            if (IsSensorAvailable())
            {
                // Set port settings
                SensorManager._serialPort.PortName = PortName;
                SensorManager._serialPort.BaudRate = BaudRate;
                try
                {
                    SensorManager._serialPort.Open();
                }
                catch
                {
                    Console.WriteLine("Could not establish connection!");
                    // Overwrite current _serialPort with new port (This causes the "SerialPort.GetPortNames();" to reset)
                    SensorManager._serialPort = new SerialPort();
                    return; // return if failed
                }

                SensorManager.IsRunning = true;

                // Initialize a new Thread that keeps listening on the _serialPort
                SensorManager._listenThread = new Thread(() =>
                {
                    Console.WriteLine("Sensor Manager Started");

                    while (true)
                    {
                        if (SensorManager.IsRunning)
                        {
                            SensorManager.ReadSensorData();
                        }
                    }
                });

                // Start the Listening thread
                SensorManager._listenThread.Start();
            }
        }

        /// <summary>
        /// Stops the _listenThread an closes the port 
        /// </summary>
        public static void Stop()
        {
            Console.WriteLine("Sensor Manager Stopped");
            SensorManager._serialPort.Close();
            SensorManager.IsRunning = false;
            SensorManager._isPressed = false;

            // Start the Listening thread
            SensorManager._listenThread.Abort();
        }

        /// <summary>
        /// Add listiner to the listiner list, each listiner sould contain a void Output() method
        /// </summary>
        /// <param name="listener">Listiner object</param>
        public static void AddListener(SensorListener listener)
        {
            SensorManager._listeners.Add(listener);
        }

        /// <summary>
        /// Removes listiner from _listiners
        /// </summary>
        /// <param name="listener">Listiner object</param>
        public static void RemoveListener(SensorListener listener)
        {
            if (SensorManager._listeners.Contains(listener))
            {
                SensorManager._listeners.Remove(listener);
            }
        }

        /// <summary>
        ///  Pause the current listiner
        /// </summary>
        public static void Pause()
        {
            SensorManager.IsRunning = !SensorManager.IsRunning;
        }

        /// <summary>
        /// Formats the given data string into 2 vars sensorName<string> & sensorPressure<int>
        /// </summary>
        /// <param name="input">String with "sensorname-pressure"</param>
        /// <returns>Return SensorData</returns>
        private static SensorData GetFormatedData(string input)
        {
            // Split string on devider "-"
            string[] info = input.Split('-');
            string sensorName = info[0]; // Sensor name 
            int sensorPressure = 0;

            // if outpur is no longer than 5 we then parse
            // (Prevents outofbounds bug!)
            if (info[1].Length < 5)
            {
                sensorPressure = Int32.Parse(info[1]); // sensor pressure
            }

            // Return SensorData with contain sensor name and pressure
            return new SensorData(sensorName, sensorPressure);
        }

        /// <summary>
        /// Checks if the port that is set in "SerialListenPort" is availible thus connected
        /// This is a Internal Check if the comper can listen on the port. 
        /// </summary>
        /// <returns>True if serial port is availible, False if not </returns>
        public static bool IsSensorAvailable()
        {
            // Get a list of serial port names. (Returns String Array)
            string[] ports = SerialPort.GetPortNames();
            
            // If port is found in the current SerialPorts list then return true
            if (ports.Contains(SensorManager.PortName))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reads data from the serial port, gets the formated data and check if the pressure is higher then the threshold
        /// if its met it will output the data to all listeners.
        /// </summary>
        private static void ReadSensorData()
        {
            // Default result empty String
            string result = string.Empty;

            // If SensorManager is running then try to read serialPort
            if (SensorManager._serialPort.IsOpen)
            {
                try
                {
                    // Read result from serailPort
                    result = SensorManager._serialPort.ReadLine();
                }
                catch
                {
                    // Stop sensor from listening when failed (Sensor disconnected)
                    SensorManager.Stop();
                }
            }

            // If result is found then format data and pass formatted data to all listiners
            if (!string.IsNullOrEmpty(result))
            {
                // Convert result to SensorData Format
                SensorData data = SensorManager.GetFormatedData(result);

                // If the pressure is higher than the Treshold (Pressure filter, sould be higher than Threshold),
                // pass to all listiners
                if (data.Pressure >= SensorManager.Threshold && !SensorManager._isPressed)
                {
                    SensorManager._isPressed = true;
                    // Call for each listiner the Output method and pass SensorData
                    foreach (SensorListener listener in SensorManager._listeners)
                    {
                        listener.Output(data);
                    }
                }
                // Set _isPressed to false when data is recived and _isPressed is true
                else if (data.Pressure < SensorManager.Threshold && SensorManager._isPressed)
                {
                    SensorManager._isPressed = false;
                }
            }
            // Sleep the thread with the given miliseconds,
            // this lowers the listen interval
            Thread.Sleep(SensorManager.Wait);
        }
    }
}
