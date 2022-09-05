using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugText : MonoBehaviour
{
    public TMP_Text text;
    public GameObject heartRateReader;

    private float heartRate;
    private float chaseDuration;
    private float FOV;
    private int checkChance;
    private float walkingSpeed;
    private float runningSpeed;
    private float searchingSpeed;

    // Update is called once per frame
    void Update()
    {
        heartRate = heartRateReader.GetComponent<HeartReader>().
            currentHR;
        chaseDuration = GetComponent<EntityController>().
            chaseDuration;
        FOV = GetComponent<EntityController>().
            fov;
        checkChance = GetComponent<EntityController>().
            hidableCheckChance;
        walkingSpeed = GetComponent<EntityController>().
            currentWalkingSpeed;
        runningSpeed = GetComponent<EntityController>().
            currentRunningSpeed;
        searchingSpeed = GetComponent<EntityController>().
            currentSearchingSpeed;


        text.text = 
            "Player Heart Rate = " + heartRate +"\n" +
            "Entity Chase Duration = " + chaseDuration +"\n" +
            "Entity FOV = " + FOV +"\n" +
            "Entity Hidable Check Rate= " + checkChance + "\n" +
            "Entity Walking Speed= " + walkingSpeed + "\n" +
            "Entity Running Speed= " + runningSpeed + "\n" +
            "Entity Searching Speed= " + searchingSpeed + "\n";
    }
}
