using Assets.Scripts.Combat.Projectiles;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class CombatController : MonoBehaviour
    {
        private Transform _transform;
        private Transform _viewTransform;

        public GameObject Projectile;

        public Camera PlayerView;

        public void Start()
        {
            _transform = transform;
            if (PlayerView != null) _viewTransform = PlayerView.transform;
        }

        public void Update()
        {
            GetInput();
        }

        private void GetInput()
        {
            if (Input.GetButtonUp("Fire1"))
            {
                var projectileObject = (GameObject)Instantiate(Projectile, _viewTransform.TransformPoint(Vector3.forward - Vector3.up / 2f), _viewTransform.rotation);

                var projectileBehavior = projectileObject.GetComponent<Rock>();
                projectileBehavior.OwnerTransform = _viewTransform;
            }
        }

        private Vector3 GetCameraCenter()
        {
            return new Vector3(Screen.width / 2, Screen.height / 2, PlayerView.nearClipPlane);
        }
    }
}