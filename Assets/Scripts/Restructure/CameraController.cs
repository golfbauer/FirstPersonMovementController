using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform PlayerTransform { get; set; }
    public float MouseSensitivity { get; set; }

    private float xRotation;
    private float zRotation;

    private bool isTiltingCamera;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        CameraLook();
    }

    void CameraLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, zRotation);
        PlayerTransform.Rotate(Vector3.up * mouseX);
    }

    public void TiltCamera(float targetTitl, float time)
    {
        if (!isTiltingCamera) StartCoroutine(TiltingCamera(targetTitl, time));
    }

    IEnumerator TiltingCamera(float targetTilt, float time)
    {
        isTiltingCamera = true;
        float timeElapsed = 0;
        float currentTilt = zRotation;

        while (timeElapsed < time)
        {
            zRotation = Mathf.Lerp(currentTilt, targetTilt, timeElapsed / time);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        isTiltingCamera = false;
        zRotation = targetTilt;
    }
}
