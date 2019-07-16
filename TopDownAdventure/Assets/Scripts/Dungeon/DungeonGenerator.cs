using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeon {
    public class DungeonGenerator : MonoBehaviour {
        [Header("Dungeon Configuration")]
        public Vector2 worldSize = new Vector2(4, 4);
        public int numberOfRooms = 4;

        [Header("Tilemaps")]
        public Tilemap groundTilemap;
        public Tilemap collisionTilemap;
        public RuleTile groundRuleTile;
        public Tile collisionTile;

        [Header("Game Objects")]
        public GameObject roomsParent;
        public GameObject roomPrefab;

        [Header("Dungeon Objects")]
        public GameObject townStairs;

        private Vector2 m_roomSizeInTiles = new Vector2(9, 17);
        private Room[,] m_rooms;
        private List<RoomInstance> m_roomInstanceList;
        private List<Vector2> m_takenPositions = new List<Vector2>();
        private int m_gridSizeX;
        private int m_gridSizeY;

        private void Awake() {
            m_roomInstanceList = new List<RoomInstance>();

            if(numberOfRooms >= (worldSize.x * 2) * (worldSize.y * 2)) {
                numberOfRooms = Mathf.RoundToInt((worldSize.x * 2) * (worldSize.y * 2));
            }

            m_gridSizeX = Mathf.RoundToInt(worldSize.x);
            m_gridSizeY = Mathf.RoundToInt(worldSize.y);

            GenerateLevel();
        }

        private void GenerateLevel() {
            CreateRooms();
            CreateRoomInstances();
            AddCollisions();
            InstantiateObjects();
        }

        #region ROOMS
        private void CreateRooms() {
            m_rooms = new Room[m_gridSizeX * 2, m_gridSizeY * 2];
            m_rooms[m_gridSizeX, m_gridSizeY] = new Room(Vector2.zero, Room.ERoomType.Start);
            m_takenPositions.Insert(0, Vector2.zero);
            Vector2 positionToCheck;

            // Creating Regular Rooms
            for(int i = 0; i < numberOfRooms; i++) {
                positionToCheck = GetNewPosition();

                // Creating it as an Standard Room
                m_rooms[(int) positionToCheck.x + m_gridSizeX, (int) positionToCheck.y + m_gridSizeY] = new Room(positionToCheck, Room.ERoomType.Regular);
                m_takenPositions.Insert(0, positionToCheck);
            }

            // After creating all the regular rooms, we add two more rooms the key and boss room
            // Creating Key Room
            positionToCheck = GetNewPosition();
            m_rooms[(int)positionToCheck.x + m_gridSizeX, (int)positionToCheck.y + m_gridSizeY] = new Room(positionToCheck, Room.ERoomType.KeyRoom);
            m_takenPositions.Insert(0, positionToCheck);

            // Creating Final (Boss) Room
            positionToCheck = GetNewPosition();
            m_rooms[(int)positionToCheck.x + m_gridSizeX, (int)positionToCheck.y + m_gridSizeY] = new Room(positionToCheck, Room.ERoomType.Final);
            m_takenPositions.Insert(0, positionToCheck);
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

        #region COLLISIONS
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
        #endregion COLLISIONS

        #region OBJECTS
        private void InstantiateObjects() {
            // Instantiating Stairs to go to the city...
            Instantiate(townStairs, new Vector3(-0.5f, 0.5f, 0), Quaternion.identity);
        }
        #endregion OBJECTS
    }
}
