using UnityEngine;
using System.Collections;

public class KinematicCharacterController : MonoBehaviour
{

    private float _slopeLimit;
    private float _stepOffset;
    private float _jumpingStepOffset = 0.1f;
    private float _skinWidth;
    private Vector3 _center;
    private float _height;
    private float _radius;

    private Vector3 move;

    CapsuleCollider capsuleCollider;
    Rigidbody rigibody;


    // Use this for initialization
    void Start()
	{
        rigibody = gameObject.AddComponent<Rigidbody>();
        rigibody.isKinematic = true;

        capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        capsuleCollider.height = Height;
        capsuleCollider.radius = Radius;
        capsuleCollider.center = Center;
	}

    private void FixedUpdate()
    {
        rigibody.position += move;
    }

    public void Move(Vector3 moveDirect)
    {
        move = moveDirect;
    }

    private void OnCollisionEnter(Collision collision)
    {
        for (int i = 0; i < collision.contactCount - 1; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            Debug.Log("normal before: " + normal);
            normal = new Vector3(
                (int) normal.x != 0 ? System.Math.Abs(normal.x) * -1f : 1,
                (int) normal.y != 0 ? System.Math.Abs(normal.y) * -1f : 1,
                (int) normal.z != 0 ? System.Math.Abs(normal.z) * -1f : 1
            );

            Vector3 collisionDirect = Vector3.Scale(move, normal);
            Debug.Log("move: " + move);
            Debug.Log("normal after: " + normal);
            Debug.Log("collisionDirect: " + collisionDirect);

            transform.position += collisionDirect;
        }
    }

    public float Height { get => _height; set => _height = value; }
    public float SlopeLimit { get => _slopeLimit; set => _slopeLimit = value; }
    public float JumpingStepOffset { get => _jumpingStepOffset; set => _jumpingStepOffset = value; }
    public float SkinWidth { get => _skinWidth; set => _skinWidth = value; }
    public float Radius { get => _radius; set => _radius = value; }
    public float StepOffset { get => _stepOffset; set => _stepOffset = value; }
    public Vector3 Center { get => _center; set => _center = value; }
}

