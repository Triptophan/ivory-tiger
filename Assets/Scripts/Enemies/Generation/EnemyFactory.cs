using Assets.Scripts.Enemies;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    public GameObject[] Enemies;

    public Enemy Spawn()
    {
        var index = Random.Range(0, Enemies.GetLength(0));
        var gameObject = (GameObject)(Instantiate(Enemies[index], Vector3.zero, Quaternion.identity));
        var enemy = gameObject.AddComponent<Enemy>();
        return enemy;
    }
}