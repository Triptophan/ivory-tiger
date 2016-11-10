using UnityEngine;
using Assets.Scripts.Enemies.AI;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(WanderBehavior))]
    [RequireComponent(typeof(ChaseBehavior))]
    public class Enemy : MonoBehaviour
    {
        ChaseBehavior chase;
        WanderBehavior wander;

        
        public bool Active { get { return isActiveAndEnabled; } set { gameObject.SetActive(value); } }

        public void OnEnable()
        {
            chase = GetComponent<ChaseBehavior>();
            wander = GetComponent<WanderBehavior>();
            

        }

        public void Update()
        {
            //If we're chasing, disable the wander behavior
            wander.isWandering = !chase.isChasing;

        }
    }
}
