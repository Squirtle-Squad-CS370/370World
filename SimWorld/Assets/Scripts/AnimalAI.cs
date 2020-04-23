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
    private float pathCheckDistance;

    void Start()
    {
        isIdling = true;
        isWandering = false;
        idleTimer = UnityEngine.Random.Range(minIdleTime, maxIdleTime);
    }
    
    void onDestroy()
    {
        WorldController.Instance.World.removeAnimal();
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

            SetRandomWanderPosition();
        }

        // Make sure we don't run into anything.
        CourseCorrect();
    }

    void FixedUpdate()
    {
        // Simply move towards our target position.
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    // Sets a random tile to wander to (within bounds of world map)
    private void SetRandomWanderPosition()
    {
        Vector3 dir = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        float dist = UnityEngine.Random.Range(minWanderDistance, maxWanderDistance);
        targetPosition = transform.position + (dir * dist);
        
        // If we picked a bad spot (out of map bounds, tile does not exist)
        // get a new one
        if( WorldController.Instance.GetTileAtWorldCoord(targetPosition) == null )
        {
            SetRandomWanderPosition();
        }
    }

    // This function makes sure that the path ahead of us is walkable.
    private void CourseCorrect()
    {
        // Find out how far we are from our target
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // If we are close, no need to check tiles beyond it
        if (distanceToTarget < moveSpeed)
        {
            pathCheckDistance = distanceToTarget;
        }
        // Otherwise check a good distance ahead
        else
        {
            pathCheckDistance = 1f + moveSpeed;
        }

        // Set up our vector before the loop
        Vector3 positionToCheck = transform.position;
        // For every spot ahead of us within path check distance, at 1 unit increments
        for (int i = 1; i <= pathCheckDistance; i++)
        {
            positionToCheck += transform.forward;
            Tile tileToCheck = WorldController.Instance.GetTileAtWorldCoord(positionToCheck);
            if (tileToCheck == null)
            {
                //Destroy(GetComponent<GameObject>());
                return;
            }

            // If the tile there is not walkable, change course
            if (!tileToCheck.isWalkable)
            {
                // FIXME: For now, all our Animal does to course correct
                // is pick a new random position to move to. Will need work
                // so that enemies can navigate walls. Maybe we will use A*
                // pathfinding for them instead and leave the animals simple?
                SetRandomWanderPosition();
                break;
            }
        }
    }
}
