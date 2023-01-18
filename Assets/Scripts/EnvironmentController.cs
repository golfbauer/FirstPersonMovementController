using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System.Collections;
using System.Drawing;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

public class EnvironmentController : MonoBehaviour
{
    [SerializeField] private bool movement;
    [SerializeField] private Vector3 startTransform;
    [SerializeField] private Vector3 targetTransform;
    [SerializeField] private float transitionTime;
    [SerializeField] private bool rotation;
    [SerializeField] private RotationDirection rotationDirection = new RotationDirection();
    [SerializeField] private float degreesPerSecond;

    public Vector3 StartTransform
    {
        get => startTransform;
        set => startTransform = value;
    }
    public Vector3 TargetTransform
    {
        get => targetTransform;
        set => targetTransform = value;
    }
    public float TransitionTime
    {
        get => transitionTime;
        set => transitionTime = value;
    }
    public bool Movement
    {
        get => movement;
        set => movement = value;
    }

    public Vector3 MovementDelta
    {
        get { return transform.position - lastPosition; }
    }

    public Vector3 RotationPoint
    {
        get { return GetComponent<Renderer>().bounds.center; }
    }

    public float DegreesPerSecond
    {
        get { return (rotationDirection == RotationDirection.Clockwise ? 1 : -1) * degreesPerSecond; }
        set { degreesPerSecond = value; }
    }

    public bool isMoving
    {
        get { return movement; }
    }

    public bool isRotating
    {
        get { return rotation; }
    }
    private enum Direction
    {
        Forward,
        Backward
    }

    private enum RotationDirection
    {
        Clockwise,
        Counterclockwise
    }

    private float timeSinceLastDirectionChange;
    private float journeyFraction;
    private Vector3 startPositionRotationPoint;
    private Vector3 targetPositionRotationPoint;
    private Direction direction;
    private Vector3 lastPosition;


    // Use this for initialization
    void Start()
    {
        timeSinceLastDirectionChange = 0;
        journeyFraction = 0;

        direction = Direction.Forward;
        lastPosition = transform.position;

        startPositionRotationPoint = RotationPoint;
        targetPositionRotationPoint = targetTransform + (startPositionRotationPoint - startTransform);
    }

    // Update is called once per frame
    void Update()
    {

        if (rotation)
        {
            transform.RotateAround(RotationPoint, Vector3.up, DegreesPerSecond * Time.deltaTime);
            startTransform = Quaternion.Euler(0, DegreesPerSecond * Time.deltaTime, 0) * (startTransform - startPositionRotationPoint) + startPositionRotationPoint;
            targetTransform = Quaternion.Euler(0, DegreesPerSecond * Time.deltaTime, 0) * (targetTransform - targetPositionRotationPoint) + targetPositionRotationPoint;
        }

        lastPosition = transform.position;

        if (movement)
        {
            timeSinceLastDirectionChange += Time.deltaTime;
            journeyFraction = timeSinceLastDirectionChange / transitionTime;

            if (direction == Direction.Forward)
            {
                transform.position = Vector3.Lerp(startTransform, targetTransform, journeyFraction);
            }
            else if (direction == Direction.Backward)
            {
                transform.position = Vector3.Lerp(targetTransform, startTransform, journeyFraction);
            }

            if (journeyFraction >= 1)
            {
                journeyFraction = 0;
                timeSinceLastDirectionChange = 0;
                direction = direction == Direction.Forward ? Direction.Backward : Direction.Forward;
            }
        }





    }
}


