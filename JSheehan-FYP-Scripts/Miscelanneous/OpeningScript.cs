using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningScript : MonoBehaviour
{
    public GameObject door;
    private bool isClosed = true;

    private void OnTriggerEnter(Collider other)
    {
        if(isClosed && (other.CompareTag("Player") || other.CompareTag("enemy")))
        {
            door.GetComponent<Animation>().Play("open");
            isClosed = false;
        }
    }
}
