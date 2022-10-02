using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
	private CharacterController controller;
	private float moveSpeed;

	void Update()
	{
		float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

		Vector3 moveDirect = transform.right * moveX + transform.forward * moveZ;

		controller.Move(moveDirect * moveSpeed * Time.deltaTime);
    }

	public void SetMoveSpeed(float moveSpeed)
    {
		this.moveSpeed = moveSpeed;
    }

	public void SetController(CharacterController controller)
    {
		this.controller = controller;
    }
}

