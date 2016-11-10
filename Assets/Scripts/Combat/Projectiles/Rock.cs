using UnityEngine;
using Assets.Scripts.Enemies;

namespace Assets.Scripts.Combat.Projectiles
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class Rock : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            var enemy = collision.gameObject.GetComponent<Enemy>();

            if (enemy == null) return;

            Destroy(this);
        }
    }
}
