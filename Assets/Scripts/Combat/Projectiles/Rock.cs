using UnityEngine;

namespace Assets.Scripts.Combat.Projectiles
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class Rock : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private GameObject _gameObject;

        public float Speed = 5f;

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
            if (collisionObject.tag == "Walls")
            {
                Destroy(_gameObject);
            }
        }
    }
}