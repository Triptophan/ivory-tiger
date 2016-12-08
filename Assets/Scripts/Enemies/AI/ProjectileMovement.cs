using UnityEngine;

namespace Assets.Scripts.Enemies
{
    public class ProjectileMovement : MonoBehaviour
    {
        private GameObject _gameObject;
        private Rigidbody _rigidbody;

        public float Speed = 150.0f;

        public float DamageValue = 10f;

        public Transform OwnerTransform;

        private void Start()
        {
            _gameObject = gameObject;
            _rigidbody = GetComponent<Rigidbody>();

            _rigidbody.velocity = OwnerTransform.TransformDirection(Vector3.forward * Speed);

            Destroy(_gameObject, 5f);
        }

        public void OnCollisionEnter(Collision collision)
        {
            var collisionObject = collision.gameObject;
            if (collisionObject.tag == "Player")
            {
                var playerHealth = collisionObject.GetComponent<PlayerHealth>();
                playerHealth.DoDamage(DamageValue);
                SelfDestruct();
            }
            if (collisionObject.tag == "Walls")
            {
                SelfDestruct();
            }
        }

        private void SelfDestruct()
        {
            Destroy(_gameObject);
        }
    }
}