using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class InputController : MonoBehaviour
{
    private bool isCrouching = false;
    public  bool isHiding = false;
    private float crouchedHeight;
    private float standingHeight;
    private float crouchedWalkSpeed = 3;
    private float croucheRunningSpeed = 6;
    private float standingWalkSpeed = 5;
    private float standingRunningSpeed = 10;
    public GameObject entity;
    public GameObject globalVol;

    public GameObject canvas;

    public GameObject child;

    private GameObject closestSpot;

    private float hideRange = 5f;


    private bool isActive = true;
    CharacterController characterController;

    private bool canvasActive = false;
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        standingHeight = characterController.height;
        
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (isCrouching)
            {
                isCrouching = false;
                characterController.height = standingHeight;
                GetComponent<FirstPersonController>().m_WalkSpeed = standingWalkSpeed;
                GetComponent<FirstPersonController>().m_RunSpeed = standingRunningSpeed;

            }
            else
            {
                isCrouching = true;
                characterController.height = crouchedHeight;
                GetComponent<FirstPersonController>().m_WalkSpeed = crouchedWalkSpeed;
                GetComponent<FirstPersonController>().m_RunSpeed = croucheRunningSpeed;

            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (canvasActive)
            {
                canvas.SetActive(false);
                canvasActive = false;
            }
            else
            {
                canvas.SetActive(true);
                canvasActive = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SceneManager.LoadScene("Home");
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (isHiding)
            {
                Hide();
            }
            else
            {
                closestSpot = findClosestHidingSpot();
                entity.GetComponent<EntityController>().setSeenHiding(closestSpot);

                if (Vector3.Distance(closestSpot.transform.position, transform.position) < hideRange)
                {
                    Hide();
                }
            }      
        }
    }

    private GameObject findClosestHidingSpot()
    {
        List<GameObject> hidingSpots = GameObject.FindGameObjectsWithTag("Hidable").ToList<GameObject>();
        GameObject foundSpot = hidingSpots[0];

        for(int i = 1; i< hidingSpots.Count; i++)
        {
            if (Vector3.Distance(hidingSpots[i].transform.position, transform.position) 
                < Vector3.Distance(foundSpot.transform.position, transform.position)
                )
            {
                foundSpot = hidingSpots[i];
            }
        }
        return foundSpot;
    }

    private void Hide()
    {

        isHiding = !isHiding;
        TogglePlayerActivity();
        closestSpot.GetComponent<ToggleCam>().SwitchCam();
    }

    private void TogglePlayerActivity()
    {
        isActive = !isActive;

        GetComponent<CharacterController>().enabled = isActive;
        GetComponent<FirstPersonController>().enabled = isActive;
        child.SetActive(isActive);

        if (!isHiding)
        {
            globalVol.SetActive(false);
        }
        else
        {
            globalVol.SetActive(true);

        }
    }
}
