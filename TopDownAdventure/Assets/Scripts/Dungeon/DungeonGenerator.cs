using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeon {
    public class DungeonGenerator : MonoBehaviour {
        [Header("Tilemaps")]
        public Tilemap groundTilemap;
        public Tilemap collisionTilemap;
        public RuleTile groundRuleTile;
        public Tile collisionTile;

        [Header("Game Objects")]
        public GameObject roomsParent;
        public GameObject roomPrefab;

        private Vector2 m_worldSize = new Vector2(4, 4);
        private Vector2 m_roomSizeInTiles = new Vector2(9, 17);
        private Room[,] m_rooms;
        private List<RoomInstance> m_roomInstanceList;
        private List<Vector2> m_takenPositions = new List<Vector2>();
        private int m_gridSizeX;
        private int m_gridSizeY;
        private int m_numberOfRooms = 4;

        private void Awake() {
            m_roomInstanceList = new List<RoomInstance>();

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
            AddCollisions();
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
                Vector3 roomPosition = new Vector3(room.gridPosition.x * (m_roomSizeInTiles.x),
                    room.gridPosition.y * (m_roomSizeInTiles.y),
                    0);
                Vector3Int roomPositionInt = new Vector3Int(Mathf.RoundToInt(roomPosition.x), Mathf.RoundToInt(roomPosition.y), 0);

                // Instantiating the Room
                RoomInstance roomInstance = Instantiate(roomPrefab, roomPosition, Quaternion.identity).GetComponent<RoomInstance>();
                m_roomInstanceList.Add(roomInstance);
                roomInstance.transform.parent = roomsParent.transform;
                roomInstance.Setup(room.gridPosition, room.roomType, groundTilemap, groundRuleTile);
            }
        }
        #endregion

        private void AddCollisions() {
            foreach(RoomInstance roomInstance in m_roomInstanceList) {
                List<Vector3> tilePositions = roomInstance.TilePositionList;
                int count = 0;
                foreach(Vector3 position in tilePositions) {
                    AddCollisionOnTile((int) position.x, (int) position.y);
                    count++;
                }
            }
        }

        private void AddCollisionOnTile(int _x, int _y) {
            TileBase tile = groundTilemap.GetTile(new Vector3Int(_x, _y, 0));

            if (
                !groundTilemap.HasTile(new Vector3Int(_x + 1, _y, 0)) ||
                !groundTilemap.HasTile(new Vector3Int(_x - 1, _y, 0)) ||
                !groundTilemap.HasTile(new Vector3Int(_x, _y + 1, 0)) ||
                !groundTilemap.HasTile(new Vector3Int(_x, _y - 1, 0)) ||

                !groundTilemap.HasTile(new Vector3Int(_x - 1, _y - 1, 0)) ||
                !groundTilemap.HasTile(new Vector3Int(_x - 1, _y + 1, 0)) ||
                !groundTilemap.HasTile(new Vector3Int(_x + 1, _y + 1, 0)) ||
                !groundTilemap.HasTile(new Vector3Int(_x + 1, _y - 1, 0))
                ) {
                collisionTilemap.SetTile(new Vector3Int(_x, _y, 0), collisionTile);
            }
        }
    }
}
