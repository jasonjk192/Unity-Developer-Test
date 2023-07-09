using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float distance = 10.0f;
    public float height = 5.0f;
    public float turnSpeed = 4.0f;

    void LateUpdate()
    {
        if (!target) return;

        // Camera will be at a certain distance and height from target (It is also independednt of the orientation of the target and the camera is user controlled)
        Vector3 pos = target.position - (transform.forward * distance) + (transform.up * height);
        transform.position = pos; 

        // Manipulate camera using mouse (only allows left-right movement)
        float mouseX = Input.GetAxis("Mouse X") * turnSpeed;
        transform.Rotate(Vector3.up, mouseX);
        
    }
}
