using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCustomization : MonoBehaviour
{
    public Transform target;
    public float moveSpeed;


    private void Update()
    {
        transform.LookAt(target);
        var HorizontalAxis = Input.GetAxis("Horizontal") * moveSpeed;
        transform.RotateAround(target.position, Vector3.up, HorizontalAxis);
    }
}
