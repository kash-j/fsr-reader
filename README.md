# fsr-reader
A Force Sensor Resistor written in C# (this was used for a school project)

## Installation

Build the project to a DLL file and import it to your C# project

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

## License
[MIT](https://choosealicense.com/licenses/mit/)
