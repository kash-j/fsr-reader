# fsr-reader
A Force Sensor Resistor written in C# (this was used for a school project)

## Installation

You will need an Arduino to run this code, the Arduino file is in this repo listed as `pressuresensor.ino`.

- Change (if needed) the pins in the `pressuresensor.ino` file
- Change (if needed) the minimum pressure in the `pressuresensor.ino` file (NOTE: You can also change it to 0 and change the threshold in your C# project)
- Build the project "SensorEvents" and add the "SensorEvents.dll" to your C# Project

## Usage
Add for example a Controller class as a listener with `SensorManager.AddListener`, example class:
```csharp
public class Controller : SensorListener
{
    /// <summary>
    /// Constructor
    /// </summary>
    public Controller()
    {
        // Port where that the controller checks if it exist so it can connect to it
        SensorManager.PortName = "COM3";
        SensorManager.AddListener(this);
        //this is the minimum force before it gets detected
        SensorManager.Threshold = 800; // Max resisting pressure is 1000

        // Only Start the SensorManagaer with listening for the sensor if serialPort is connected,
        // and only start when the Static manager is not running
        if (SensorManager.IsSensorAvailable())
        {
            if (SensorManager.IsRunning)
            {
                SensorManager.Stop();
            }
            SensorManager.Start();
        }
    }

    /// <summary>
    /// Output - Retrieves data from SensorEvents
    /// </summary>
    /// <param name="data">SensorEvent Data</param>
    public void Output(SensorData data)
    {
        Console.WriteLine($"Name: {data.Name} - Pressure: {data.Pressure}");
    }
}
```

NOTE: The `ForceSensorApplication` also has a test class inside of it.

## License
[MIT](https://choosealicense.com/licenses/mit/)
