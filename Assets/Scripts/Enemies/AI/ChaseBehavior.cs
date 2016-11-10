using UnityEngine;

namespace Assets.Scripts.Enemies.AI
{
    public class ChaseBehavior : MonoBehaviour
    {
        public bool isChasing = false;
        public float playerDetectionRadius = 20f;
        private NavMeshAgent agent;
        //private LevelManager levelManager;
        private bool hadPlayer = false;
        Vector3 initialPosition;

        private void OnEnable()
        {
            agent = GetComponent<NavMeshAgent>();
            //Save off initial position
            initialPosition = transform.position;
        }

        public void Update()
        {
            var x = Physics.OverlapSphere(transform.position, playerDetectionRadius);
            foreach (var z in x)
            {
               if(z.GetType() == typeof(CharacterController))
                {
                    //We have player
                    isChasing = true;
                    hadPlayer = true;

                    var playerPosition = z.transform.position;
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
                        hadPlayer = false;
                        agent.SetDestination(initialPosition);
                    }
                }
            }
        }
    }
}
