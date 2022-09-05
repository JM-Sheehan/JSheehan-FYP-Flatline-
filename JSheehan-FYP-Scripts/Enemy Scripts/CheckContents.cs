using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckContents : MonoBehaviour
{
    public List<GameObject> contents;
    public List<GameObject> hidingSpaces;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PatrolPoint")){
            if(contents.Contains(other.gameObject) ==  false) contents.Add(other.gameObject);
        }
        if (other.CompareTag("Hidable"))
        {
            if (hidingSpaces.Contains(other.gameObject) == false) hidingSpaces.Add(other.gameObject);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        contents.Remove(other.gameObject);
    }
}
