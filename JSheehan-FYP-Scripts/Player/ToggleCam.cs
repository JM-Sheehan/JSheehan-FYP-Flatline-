using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCam : MonoBehaviour
{
    public GameObject cam;
    private bool camActive = false;
    public void SwitchCam()
    {
        print("Here");
        camActive = !camActive;
        cam.SetActive(camActive);
    }
}
