using Assets.Scripts.Enemies.AI;
using UnityEngine;
using Assets.Scripts.Combat.Projectiles;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(WanderBehavior))]
    [RequireComponent(typeof(ChaseBehavior))]
    public class Enemy : MonoBehaviour
    {
        private ChaseBehavior _chase;
        private WanderBehavior _wander;
        private GameObject _gameObject;

        public bool Active { get { return isActiveAndEnabled; } set { gameObject.SetActive(value); } }

        public void Start()
        {
            _gameObject = gameObject;
            _gameObject.tag = "Enemy";
            _chase = GetComponent<ChaseBehavior>();
            _wander = GetComponent<WanderBehavior>();
        }

        public void Update()
        {
            //If we're chasing, disable the wander behavior
            _wander.isWandering = !_chase.isChasing;
        }

        public void OnCollisionEnter(Collision collision)
        {
            var collisionObject = collision.gameObject;
            if (collisionObject.tag == "Walls" || collisionObject.tag == "Enemy") return;

            var rockBehavior = collisionObject.GetComponent<Rock>();
            if(rockBehavior != null)
            {
                Destroy(collisionObject);
                Destroy(_gameObject);
            }
        }
    }
}