using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class HeartReader : MonoBehaviour
{
    public float intervalTime = 1f;
    private float currentInterval = 0f;
    SerialPort port = new SerialPort("COM5", 9600);
    public float currentHR = 0;
    private void Start()
    {
        port.Open();

    }

    public float GetDifferential;

    private void Update()
    {
        currentInterval += Time.deltaTime;
        if(currentInterval > intervalTime)
        {
            if (port.IsOpen)
            {
                string result = port.ReadLine();
                if (result[0] == 'B')
                {
                    result = result.Replace("BPM: ", "");
                    currentHR = Int32.Parse(result);
                    GetComponent<HeartRateController>().AddHeartRate(Int32.Parse(result));

                }
            }
            currentInterval = 0f;
        }

    }
}