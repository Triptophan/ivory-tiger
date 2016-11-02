using UnityEngine;

namespace Assets.Scripts
{
    public class LevelManager : MonoBehaviour
    {
        public MapGenerator MapGenerator;

        public GameObject PlayerPrefab;

        private void Start()
        {
            MapGenerator.GenerateMap();

            var mainRoom = MapGenerator.Rooms[0];
            var roomPosition = mainRoom.GetRoomCenter(MapGenerator.Width, MapGenerator.Height);
            var playerPosition = new Vector3(roomPosition.x, MapGenerator.PlayerStartingY, roomPosition.z);

            Instantiate(PlayerPrefab, playerPosition, Quaternion.identity);
        }
    }
}
