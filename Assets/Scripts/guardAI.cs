using UnityEngine;
using UnityEngine.AI;

public class guardAI : MonoBehaviour
{
    [SerializeField] private float chaseDistance = 5;
    [SerializeField] private float triggerDistance = 1;
    [SerializeField] private float timeSinceLastSawPlayer = Mathf.Infinity;
    [SerializeField] private PatrolPath patrolPath;
    [SerializeField] float wayPointTolerance = 1f;
    [SerializeField] float timeAtWaypoint = Mathf.Infinity;
    [SerializeField] float dwellTime = 3f;
    
    [Range(0,6)]
    [SerializeField] float patrolSpeed = 3f;

    int currentWaypointIndex = 0;
    Vector3 startingPos;
    Vector3 playerPos;
    GameObject player;
     NavMeshAgent agent;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        startingPos = transform.position;
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        playerPos = player.transform.position;
        Chase(playerPos);
    }

    private void Chase(Vector3 playerPos)
    {
        if (visionRange(playerPos))
            {
                Attack();
            }
            else if (timeSinceLastSawPlayer < 5)
            {
                Suspicion();
            }
            else
            {
                agent.speed = patrolSpeed;
                Patrol();
            }

            timeSinceLastSawPlayer += Time.deltaTime;
            timeAtWaypoint += Time.deltaTime;
    }
    private void Patrol()
    {
        Vector3 nextPosition = startingPos;
        if (patrolPath != null)
        {
            if (AtWaypoint())
            {
                timeAtWaypoint = 0;
                CycleWaypoint();
            }
            nextPosition = GetCurrentWaypoint();
        }
        if (timeAtWaypoint > dwellTime)
        {
            agent.destination = nextPosition;
        }
    }

    private void Suspicion()
    {
        //Implement additional behavior to look for player or not
    }

    private void Attack()
    {
        timeSinceLastSawPlayer = 0;
        agent.destination = player.transform.position;
        
        if (encounterRange(playerPos))
        {
            //Trigger battle encounter
            agent.speed = 0;
        }
    }

   private bool AtWaypoint()
    {
        float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
        return distanceToWaypoint < wayPointTolerance;
    }

    private void CycleWaypoint()
    {
        currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
    }

    private Vector3 GetCurrentWaypoint()
    {
        return patrolPath.GetWaypoint(currentWaypointIndex);
    }
    private bool visionRange(Vector3 playerPos)
    {
        return Vector3.Distance(transform.position, playerPos) <= chaseDistance;
    }
     private bool encounterRange(Vector3 playerPos)
    {
        return Vector3.Distance(transform.position, playerPos) <= triggerDistance;
    }
}
