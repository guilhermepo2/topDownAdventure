using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeon {
    public class DungeonGenerator : MonoBehaviour {
        [Header("Tilemaps")]
        public Tilemap groundTilemap;
        public Tile groundTile;
        public RuleTile groundRuleTile;

        public GameObject roomsParent;
        public GameObject roomPrefab;

        private Vector2 m_worldSize = new Vector2(4, 4);
        public Vector2 roomSizeInTiles = new Vector2(9, 17);
        private Room[,] m_rooms;
        private List<Vector2> m_takenPositions = new List<Vector2>();
        private int m_gridSizeX;
        private int m_gridSizeY;
        private int m_numberOfRooms = 10;

        private void Awake() {
            if(m_numberOfRooms >= (m_worldSize.x * 2) * (m_worldSize.y * 2)) {
                m_numberOfRooms = Mathf.RoundToInt((m_worldSize.x * 2) * (m_worldSize.y * 2));
            }

            m_gridSizeX = Mathf.RoundToInt(m_worldSize.x);
            m_gridSizeY = Mathf.RoundToInt(m_worldSize.y);

            GenerateLevel();
        }

        private void GenerateLevel() {
            CreateRooms();
            CreateRoomInstances();

            // Check Tiles and Add Borders
            TileBase[] allTiles = groundTilemap.GetTilesBlock(groundTilemap.cellBounds);
            Debug.Log($"{allTiles.Length}");
            foreach(TileBase _tile in allTiles) {
                Debug.Log($"tile: {_tile}");
            }
            // Check Tiles and Add Collisions
        }

        #region ROOMS
        private void CreateRooms() {
            m_rooms = new Room[m_gridSizeX * 2, m_gridSizeY * 2];
            m_rooms[m_gridSizeX, m_gridSizeY] = new Room(Vector2.zero, Room.ERoomType.Start);
            m_takenPositions.Insert(0, Vector2.zero);
            Vector2 positionToCheck;

            float randomToCompare;
            float randomToCompareStart = 0.2f;
            float randomToCompareEnd = 0.1f;

            // Creating Rooms
            for(int i = 0; i < m_numberOfRooms; i++) {
                float percentage = ( (float) i / (float) (m_numberOfRooms-1));
                randomToCompare = Mathf.Lerp(randomToCompareStart, randomToCompareEnd, percentage);
                positionToCheck = GetNewPosition();

                // Creating it as an Standard Room
                m_rooms[(int) positionToCheck.x + m_gridSizeX, (int) positionToCheck.y + m_gridSizeY] = new Room(positionToCheck, Room.ERoomType.Regular);
                m_takenPositions.Insert(0, positionToCheck);
            }
        }

        private Vector2 GetNewPosition() {
            int x;
            int y;
            Vector2 positionToCheck;
            int index;

            do {
                // Selecting a Random Room to Branch From
                index = Mathf.RoundToInt(Random.value * (m_takenPositions.Count - 1));
                x = Mathf.RoundToInt(m_takenPositions[index].x);
                y = Mathf.RoundToInt(m_takenPositions[index].y);

                // deciding if we should branch up or down
                bool isUpOrDown = Random.value < 0.5f;
                bool isPositiveOrNegative = Random.value < 0.5f;

                if(isUpOrDown) {
                    if(isPositiveOrNegative) {
                        y++;
                    } else {
                        y--;
                    }
                } else {
                    if(isPositiveOrNegative) {
                        x++;
                    } else {
                        x--;
                    }
                }

                positionToCheck = new Vector2(x, y);
            } while (m_takenPositions.Contains(positionToCheck) ||
                    x >= m_gridSizeX ||
                    x < -m_gridSizeX ||
                    y >= m_gridSizeY ||
                    y < -m_gridSizeY);

            return positionToCheck;
        }
        #endregion ROOMS


        #region ROOMS INSTANCES
        private void CreateRoomInstances() {
            foreach(Room room in m_rooms) {
                if(room == null) {
                    continue;
                }

                // Finding where the room will be
                Vector3 roomPosition = new Vector3(room.gridPosition.x * (roomSizeInTiles.x),
                    room.gridPosition.y * (roomSizeInTiles.y),
                    0);
                Vector3Int roomPositionInt = new Vector3Int(Mathf.RoundToInt(roomPosition.x), Mathf.RoundToInt(roomPosition.y), 0);

                // Instantiating the Room
                RoomInstance roomInstance = Instantiate(roomPrefab, roomPosition, Quaternion.identity).GetComponent<RoomInstance>();
                roomInstance.transform.parent = roomsParent.transform;
                roomInstance.Setup(room.gridPosition, room.roomType, groundTilemap, groundRuleTile);
            }
        }
        #endregion
    }
}
