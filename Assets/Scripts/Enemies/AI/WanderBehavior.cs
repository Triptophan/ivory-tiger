using UnityEngine;

namespace Assets.Scripts.Enemies.AI
{
    public class WanderBehavior : MonoBehaviour
    {
        public float wanderRadius = 5f;
        public float wanderTimer = 5f;
        public bool isWandering = true;

        private Transform target;
        private NavMeshAgent agent;
        private float timer;

        // Use this for initialization
        private void OnEnable()
        {
            agent = GetComponent<NavMeshAgent>();
            timer = wanderTimer;
        }

        // Update is called once per frame
        private void Update()
        {
            if (!isWandering)
                return;

            timer += Time.deltaTime;

            if (timer >= wanderTimer)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                timer = 0;
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
}