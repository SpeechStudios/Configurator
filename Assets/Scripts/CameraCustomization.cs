using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCustomization : MonoBehaviour
{
    public Transform target;
    public float moveSpeed;
    public float MouseFoce = 800f;



    private void Update()
    {
        transform.LookAt(target);
        var HorizontalAxis = Input.GetAxis("Horizontal") * -moveSpeed;
        transform.RotateAround(target.position, Vector3.up, HorizontalAxis);
        if (Input.GetButton("Fire1"))
        {
            float verticalInput = Input.GetAxis("Mouse X") * MouseFoce * Time.deltaTime;
            transform.RotateAround(target.position, Vector3.up, verticalInput);
        }

    }
}
