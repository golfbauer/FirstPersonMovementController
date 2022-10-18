using UnityEngine;
using System.Collections;

public class PlayerCameraLook : MonoBehaviour
{

	private Transform playerTransform;

	private float mouseSensitivity;

	private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
	{
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);

		transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerTransform.Rotate(Vector3.up * mouseX);

    }

    public void SetPlayerTransform(Transform playerTransform)
    {
        this.playerTransform = playerTransform;
    }

	public void SetMouseSensitivity(float mouseSensitivity)
    {
		this.mouseSensitivity = mouseSensitivity;
    }

}

