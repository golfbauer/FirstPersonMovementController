using UnityEngine;
using System.Collections;

public class PlayerCameraLook : MonoBehaviour
{

    public Transform PlayerTransform { get; set; }
	public float MouseSensitivity { get; set; }

    public bool TiltCameraRight { get; set; }
    public bool CameraTiltedLeft = false;
    public bool CameraTiltedRight = false;
    public float TimeToTiltCameraWallRun { get; set; }
    public float MaxCameraTilt { get; set; }

	private float xRotation = 0f;
    private float zRotation = 0f;
    private bool isTiltingCamera;

    private float bobTimer = 0f;
    private float defaultPositionY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        defaultPositionY = transform.localPosition.y;
    }

    void Update()
	{
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);

		transform.localRotation = Quaternion.Euler(xRotation, 0f, zRotation);
        PlayerTransform.Rotate(Vector3.up * mouseX);

    }

    public void TiltRight()
    {
        if (CameraTiltedRight) return;
        TiltCameraRight = true;
        TiltCamera();
    }

    public void TiltLeft()
    {
        if (CameraTiltedLeft) return;
        TiltCameraRight = false;
        TiltCamera();
    }

    public void TiltCamera()
    {
        if(!isTiltingCamera) StartCoroutine(TiltingCamera());
    }

    IEnumerator TiltingCamera()
    {
        isTiltingCamera = true;
        float timeElapsed = 0;
        float targetTilt = CameraTiltedLeft || CameraTiltedRight ? 0f : TiltCameraRight ? -MaxCameraTilt : MaxCameraTilt;
        float currentTilt = zRotation;

        while (timeElapsed < TimeToTiltCameraWallRun)
        {
            zRotation = Mathf.Lerp(currentTilt, targetTilt, timeElapsed / TimeToTiltCameraWallRun);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        isTiltingCamera = false;
        zRotation = targetTilt;
        if (targetTilt < 0) CameraTiltedRight = true;
        if (targetTilt > 0) CameraTiltedLeft = true;
        if (targetTilt == 0) 
        { 
            CameraTiltedLeft = false;
            CameraTiltedRight = false;
        }
    }

    public void HeadBobCamera(float timer, float bobAmount)
    {
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            defaultPositionY + Mathf.Sin(timer) * bobAmount,
            transform.localPosition.z);
    }
}

