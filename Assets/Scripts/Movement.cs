using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{

	private float moveSpeed;

	void Update()
	{
		float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

		Vector3 moveDirect = transform.right * moveX + transform.forward * moveZ;

		transform.position += moveDirect * moveSpeed * Time.deltaTime;
    }

	public void SetMoveSpeed(float moveSpeed)
    {
		this.moveSpeed = moveSpeed;
    }
}

