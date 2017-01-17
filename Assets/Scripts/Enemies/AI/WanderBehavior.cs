using UnityEngine;

public class WanderBehavior : MonoBehaviour
{
    public float wanderRadius = 5f;
    public float wanderTimer = 5f;

    public int maxWanderMoves = 5;
    public bool isWandering = true;

    private Transform mob;

    //Start Position of the mob.
    private Vector3 startPosition;

    private NavMeshAgent agent;
    private float timer;
    private int numberOfMoves = 0;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        mob = transform;
        startPosition = mob.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isWandering)
            return;

        timer += Time.deltaTime;

        if (timer >= wanderTimer && agent.remainingDistance == 0.0f)
        {
            if (numberOfMoves == maxWanderMoves)
            {
                agent.SetDestination(startPosition);
                numberOfMoves = 0;
            }
            else
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                timer = 0;
                numberOfMoves++;
            }
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
}