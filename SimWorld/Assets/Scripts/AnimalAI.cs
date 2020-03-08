using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAI : MonoBehaviour
{
    // These values are all currently set in the editor, that way we can
    // customize for different animal types' behaviors. Skyler idk if you
    // prefer to use the MAX_WANDER_DISTANCE style format instead for these,
    // if so I think that looks good too. Either way!
    [SerializeField]
    private float minWanderDistance;
    [SerializeField]
    private float maxWanderDistance;
    [SerializeField]
    private float minIdleTime;
    [SerializeField]
    private float maxIdleTime;
    [SerializeField]
    private float moveSpeed;

    private Vector3 targetPosition;
    private float idleStartTime;
    private float idleTimer;
    private bool isIdling;
    private bool isWandering;
    private 

    void Start()
    {
        isIdling = true;
        isWandering = false;
        idleTimer = UnityEngine.Random.Range(minIdleTime, maxIdleTime);
    }

    private void Update()
    {
        // If we are idling, stay here and tick the timer.
        if( isIdling )
        {
            targetPosition = transform.position;
            idleTimer -= Time.deltaTime;
        }
        // If we are not idling and we have reached our destination,
        // i.e. we have finished walking to the next wander position,
        // then idle there for a bit.
        else if( Vector3.Distance(transform.position, targetPosition) <= 0.1f )
        {
            isIdling = true;
            isWandering = false;
            idleTimer = UnityEngine.Random.Range(minIdleTime, maxIdleTime);
        }

        // If we are done idling, pick a new destination to move towards
        if( idleTimer <= 0 && ! isWandering )
        {
            isIdling = false;
            isWandering = true;
            Vector3 dir = GetRandomDirection(); // not sure if this needs its own func
            float dist = UnityEngine.Random.Range(minWanderDistance, maxWanderDistance);
            targetPosition = transform.position + (dir * dist);

            // FIXME: Need to check if tile at targetPosition is walkable. I not, pick new target
        }
    }

    void FixedUpdate()
    {
        // Simply move towards our target position.
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private Vector3 GetRandomDirection()
    {
        return new Vector3(UnityEngine.Random.Range(1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
    }

}
