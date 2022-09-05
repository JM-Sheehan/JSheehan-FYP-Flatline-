using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EntityHeartRate : MonoBehaviour
{
    float chaseDuration;
    float fov;
    int hidableCheckChance;

    float walkingSpeed;
    float searchingSpeed;
    float runningSpeed;

    EntityController controller;
    public GameObject heartRateObject;
    private float hidableCheckFloating;

    HeartReader hr;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<EntityController>();
        hidableCheckFloating = (float)controller.hidableCheckChance;
    }


    public void ModifyStats(float modifier)
    {
        if (modifier != 0)
        {
            float chaseModification = (100 + modifier) / 100;
            float fovModification = (100 + modifier) / 100;
            float hidableCheckModification = (100 - (chaseModification * 10)) / 100;


            hidableCheckFloating = hidableCheckFloating * hidableCheckModification;


            if(controller.chaseDuration * chaseModification > 8f)
            {
                controller.chaseDuration = (controller.chaseDuration * chaseModification);
            }

            controller.fov = (controller.fov * fovModification);

            if(controller.fov < 120)
            {
                controller.fov = 120;
            }

            if(controller.fov > 200)
            {
                controller.fov = 200;
            }

            if((int) hidableCheckChance > 3)
            {
                controller.hidableCheckChance = (int)hidableCheckFloating;
            }

            float movementModification = (100 + modifier) / 100;


            controller.currentWalkingSpeed = (controller.currentWalkingSpeed * movementModification);
            controller.currentSearchingSpeed = (controller.currentSearchingSpeed * movementModification);
            controller.currentRunningSpeed = (controller.currentRunningSpeed * movementModification);

            if(controller.currentWalkingSpeed < 3 && controller.currentWalkingSpeed > 5)
            {
                controller.currentWalkingSpeed = 3.5f;
            }

            if (controller.currentSearchingSpeed < 4 && controller.currentSearchingSpeed > 8)
            {
                controller.currentSearchingSpeed = 4.5f;
            }

            if (controller.currentRunningSpeed < 6 && controller.currentRunningSpeed > 12)
            {
                controller.currentRunningSpeed = 6.5f;
            }
        }
       

    }
}
