using UnityEngine;
using Assets.Scripts.Enemies.AI;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(WanderBehavior))]
    [RequireComponent(typeof(BoxCollider))]
    public class Enemy : MonoBehaviour
    {
        public bool Active
        {
            get { return isActiveAndEnabled; }
            set { gameObject.SetActive(value); }
        }
    }
}
