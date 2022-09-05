using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartRateController : MonoBehaviour
{
   
    public List<List<float>> rates = new List<List<float>>();
    int listSize = 5;
    public GameObject enemy;

    public float currentHR;

    private void CalculateDifferential()
    {
        float averageOne = Average(rates[0]);
        float averageTwo = Average(rates[1]);

        float result = averageTwo - averageOne;

        rates.RemoveAt(0);
        currentHR = result;
        enemy.GetComponent<EntityHeartRate>().ModifyStats(result);
    }
   

    private float Average(List<float> input)
    {
        float result = 0;
        foreach(float f in input)
        {
            result += f;
        }
        return result;
    }
    public void AddHeartRate(float input)
    {
        if(rates.Count == 0)
        {
            List<float> list = new List<float>();
            list.Add(input);
            rates.Add(list);
        }
        else if(rates.Count == 1)
        {
            if(rates[0].Count == 5)
            {
                List<float> newList = new List<float>();
                newList.Add(input);
                rates.Add(newList);
            }
            else
            {
                rates[0].Add(input);
            }
        }
        else if(rates.Count == 2)
        {
            if (rates[1].Count == 4)
            {
                rates[1].Add(input);
                CalculateDifferential();
            }
            else
            {
                rates[1].Add(input);
            }
        }
    }
}
