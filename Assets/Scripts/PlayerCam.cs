using System;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;
    
    [SerializeField] private Transform orientation;
    
    private float xRotation;
    private float yRotation;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;

        yRotation += mouseX;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        orientation.transform.localRotation = Quaternion.Euler(0, yRotation, 0);
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}
