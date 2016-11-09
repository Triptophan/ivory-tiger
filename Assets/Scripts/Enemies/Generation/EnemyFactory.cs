using UnityEngine;

namespace Assets.Scripts.Enemies.Generation
{
    public class EnemyFactory : MonoBehaviour
    {
        public Enemy Spawn()
        {
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var enemy = gameObject.AddComponent<Enemy>();
            gameObject.AddComponent<Rigidbody>();

            return enemy;
        }
    }
}
