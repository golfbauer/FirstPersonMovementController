using System.Collections;
using UnityEngine;


public class EnvironmentController : MonoBehaviour
{
    [SerializeField] private Vector3 startTransform;
    [SerializeField] private Vector3 targetTransform;
    [SerializeField] private float transitionTime;
    private enum Direction
    {
        Forward,
        Backward
    }

    private float timeSinceLastDirectionChange;
    private float journeyFraction;

    private Direction direction;
    private Vector3 lastPosition;

    public Vector3 MovementDelta 
    { 
        get { return transform.position - lastPosition; } 
    }  

    // Use this for initialization
    void Start()
    {
        timeSinceLastDirectionChange = 0;
        journeyFraction = 0;
        direction = Direction.Forward;  
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        lastPosition = transform.position;
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


