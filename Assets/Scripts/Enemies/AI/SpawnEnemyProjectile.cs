using UnityEngine;

namespace Assets.Scripts.Enemies
{
    public class SpawnEnemyProjectile : MonoBehaviour
    {
        public GameObject projectile;
        public float delay = 1.0f;

        public GameObject enemyParent;
        public Enemy enemyBehaviorScript;

        // Use this for initialization
        private void Start()
        {
            enemyParent = gameObject.transform.parent.gameObject;
            enemyBehaviorScript = enemyParent.GetComponentInChildren<Enemy>();
        }

        // Update is called once per frame
        private void Update()
        {
            delay = Mathf.Clamp01(delay);
            if (delay > 0.0f)
            {
                delay -= Time.deltaTime;
            }

            if (delay < 0.01f)
            {
                delay = 1.0f;
            }

            if (enemyBehaviorScript.CanAttack && delay == 1.0f)
            {
                var projectileObject = (GameObject)Instantiate(projectile, transform.position, transform.rotation);
                projectileObject.tag = "Enemy";
                var projectileBehavior = projectileObject.GetComponent<ProjectileMovement>();
                projectileBehavior.OwnerTransform = enemyParent.transform;
            }
        }
    }
}