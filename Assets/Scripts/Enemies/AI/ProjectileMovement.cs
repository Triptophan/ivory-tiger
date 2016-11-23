using UnityEngine;
using System.Collections;
namespace Assets.Scripts.Enemies
{
    public class ProjectileMovement : MonoBehaviour
    {
        public float Speed = 150.0f;
        private GameObject _gameObject;
        private Rigidbody _rigidbody;

        public Transform OwnerTransform;

        void Start()
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
