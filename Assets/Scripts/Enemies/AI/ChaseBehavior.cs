using UnityEngine;

public class ChaseBehavior : MonoBehaviour
{
    public bool isChasing = false;
    public float playerDetectionRadius = 20f;
    public float playerFoundDetectionRadiusMultiplier = 1.5f;
    private NavMeshAgent agent;

    //private LevelManager levelManager;
    private bool hadPlayer = false;

    private Vector3 initialPosition;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //Save off initial position
        initialPosition = transform.position;
    }

    private void OnEnable()
    {
    }

    public void Update()
    {
        var collisions = Physics.OverlapSphere(transform.position, playerDetectionRadius);

        foreach (var collision in collisions)
        {
            if (collision.GetType() == typeof(CharacterController))  //is the player being detected?
            {
                //We have player
                isChasing = true;
                hadPlayer = true;

                //enemy is focused now, increase the playerdetectionradius
                playerDetectionRadius = playerDetectionRadius * playerFoundDetectionRadiusMultiplier;

                var playerPosition = collision.transform.position;
                agent.SetDestination(playerPosition);
                break;
            }
            else
            {
                //Can't find player, turn off chasing
                isChasing = false;

                //We are no longer chasing the player so return to my original location
                if (hadPlayer)
                {
                    //lost the enemy, reset playerdetectionradius
                    playerDetectionRadius = playerDetectionRadius / playerFoundDetectionRadiusMultiplier;
                    hadPlayer = false;
                    agent.SetDestination(initialPosition);
                }
            }
        }
    }
}