using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
    public float spinSpeed = 10f; // speed of rotation

    // Update is called once per frame
    void Update()
    {
        // get current rotation
        Vector3 currentRotation = transform.rotation.eulerAngles;

        // increment x and y rotation by spin speed
        currentRotation.x += spinSpeed * Time.deltaTime;
        currentRotation.y += spinSpeed * Time.deltaTime;

        // set new rotation using Euler angles
        transform.rotation = Quaternion.Euler(currentRotation);
    }
}
