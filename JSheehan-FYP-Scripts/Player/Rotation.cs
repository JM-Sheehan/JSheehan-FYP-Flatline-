using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    // Update is called once per frame
    public float multiplier;
    public GameObject vol;
    float currentRotationX;
    float currentRotationY;
    float currentRotationZ;

    private void Start()
    {
        currentRotationX = transform.rotation.x;
        currentRotationY = transform.rotation.y;
        currentRotationZ = transform.rotation.z;
        vol.SetActive(true);
    }
    void Update()
    {

        currentRotationY += (Input.GetAxis("Mouse X") * multiplier);

        transform.rotation = Quaternion.Euler(currentRotationX, currentRotationY, currentRotationZ);
    }
}
