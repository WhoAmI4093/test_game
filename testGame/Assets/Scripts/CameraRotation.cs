using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    GameObject player;

    CanvasUpdate canvasUpdate;

    public static CameraRotation current;

    public float mouseSensitivity = 100f;
    float xRotation = 0f;
    //    confined = 2,
    //    locked = 1,
    //    none = 0
    
    
    void Start()
    {
        current = this;

        canvasUpdate = CanvasUpdate.current;

        player = transform.parent.gameObject;
        setCursorLock(1);
    }

    
    void Update()
    {
        if (canvasUpdate == null) canvasUpdate = CanvasUpdate.current;
        if (canvasUpdate.gamePaused) return;
        if (player == null) player = transform.parent.gameObject;
        
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.transform.Rotate(Vector3.up * mouseX);
    }

    public void setCursorLock(int number)
    {
        Cursor.lockState = (CursorLockMode)number;
    }
}
