using System.Collections.Generic;
using System.Linq;
using ExtensionMethods;
using Manager;
using UnityEngine;

/*
This script is used to read all the data coming from the device. For instance,
If arduino send ->
								{"1",
								"2",
								"3",}
readQueue() will return ->
								"1", for the first call
								"2", for the second call
								"3", for the thirst call

This is the perfect script for integration that need to avoid data loose.
If you need speed and low latency take a look to wrmhlReadLatest.
*/

public class wrmhlRead : MonoBehaviour
{
    wrmhl myDevice = new wrmhl(); // wrmhl is the bridge beetwen your computer and hardware.

    [Tooltip("SerialPort of your device.")]
    public string portName = "COM8";

    [Tooltip("Baudrate")] public int baudRate = 250000;


    [Tooltip("Timeout")] public int ReadTimeout = 20;

    [Tooltip("QueueLenght")] public int QueueLength = 1;

    private float movementSpeed = 5f;

    void Start()
    {
        myDevice.set(portName, baudRate, ReadTimeout,
            QueueLength); // This method set the communication with the following vars;
        //                              Serial Port, Baud Rates, Read Timeout and QueueLenght.
        myDevice.connect(); // This method open the Serial communication with the vars previously given.
    }

    // Update is called once per frame
    void Update()
    {
        var payload = myDevice.readQueue(); // myDevice.read() return the data coming from the device using thread.
        print(payload);

        List<string> parsed = payload.Split(',').ToList();
        float xDirection = float.Parse(parsed[0]);
        float zDirection = float.Parse(parsed[1]);


        float xAxis = xDirection.Map(0, 1023, -1, 1);
        float zAxis = zDirection.Map(0, 1023, 1, -1);

        transform.position = transform.position + new Vector3(xAxis * movementSpeed * Time.deltaTime, 0,
            zAxis * movementSpeed * Time.deltaTime);
    }

    void OnApplicationQuit()
    {
        // close the Thread and Serial Port
        myDevice.close();
    }
}