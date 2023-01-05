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
    private Coroutine cameraTiltCoroutine;

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

    public void TiltCamera(float targetTilt, float time)
    {
        if(cameraTiltCoroutine != null)
        {
            StopCoroutine(cameraTiltCoroutine);
            cameraTiltCoroutine = null;
        }

        cameraTiltCoroutine = StartCoroutine(TiltingCamera(targetTilt, time));
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

    public void AddCameraHeight(float targetHeight)
    {
        transform.position += Vector3.up * targetHeight;
    }
}
