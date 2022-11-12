using System.Collections;
using UnityEngine;


public class EnvironmentController : MonoBehaviour
{
    [SerializeField] private Vector3 startTransform;
    [SerializeField] private Vector3 targetTransform;
    [SerializeField] private float transitionTime;

    private float timeElapsedSinceLarpStart;
    private float journeyFraction;
    private enum Direction
    {
        Forward,
        Backward
    }
    private Direction direction;

    // Use this for initialization
    void Start()
    {
        timeElapsedSinceLarpStart = 0;
        journeyFraction = 0;
        direction = Direction.Forward;  
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsedSinceLarpStart += Time.deltaTime;
        journeyFraction = timeElapsedSinceLarpStart / transitionTime;

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
            timeElapsedSinceLarpStart = 0;
            direction = direction == Direction.Forward ? Direction.Backward : Direction.Forward;
        }
    }
}


