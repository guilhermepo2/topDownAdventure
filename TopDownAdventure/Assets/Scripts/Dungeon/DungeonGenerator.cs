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
        public RuleTile wallTile;

        [Header("Game Objects")]
        public GameObject roomsParent;
        public GameObject roomPrefab;
        public GameObject doorPrefab;


        [Header("Dungeon Objects")]
        public GameObject townStairs;
        public GameObject nextFloorStairs;
        public GameObject bossRoomKey;
        public GameObject enemyEncounter;
        public GameObject bossEncounter;

        [Header("Enemies in Dungeon Data")]
        public int minimumAmountOfEnemies = 10;
        public int maximumAmountOfEnemies = 20;

        private Vector2 m_roomSizeInTiles = new Vector2(8, 8);
        private Room[,] m_rooms;
        private List<Room> m_roomsList;
        private List<RoomInstance> m_roomInstanceList;
        private List<Vector2> m_takenPositions = new List<Vector2>();
        private int m_gridSizeX;
        private int m_gridSizeY;

        private void Awake() {
            m_roomInstanceList = new List<RoomInstance>();
            m_roomsList = new List<Room>();

            if(numberOfRooms >= (worldSize.x * 2) * (worldSize.y * 2)) {
                numberOfRooms = Mathf.RoundToInt((worldSize.x * 2) * (worldSize.y * 2));
            }

            m_gridSizeX = Mathf.RoundToInt(worldSize.x);
            m_gridSizeY = Mathf.RoundToInt(worldSize.y);

            GenerateLevel();
        }

        private void GenerateLevel() {
            Debug.Log("[DUNGEON GENERATOR] Creating Rooms...");
            CreateRooms();

            Debug.Log("[DUNGEON GENERATOR] Placing Goals...");
            PlaceGoalRoom();

            // Validating...
            if(IsRoomValid()) {
                Debug.Log("[DUNGEON GENERATOR] Placing Instances...");
                CreateRoomInstances();

                Debug.Log("[DUNGEON GENERATOR] Adding Collisions");
                AddCollisions();

                Debug.Log("[DUNGEON GENERATOR] Instantiating Dungeon Objects");
                InstantiateObjects();
            } else {
                ClearLevel();
                GenerateLevel();
            }
        }

        #region ROOMS
        private void CreateRooms() {
            m_rooms = new Room[m_gridSizeX * 2, m_gridSizeY * 2];
            Room starterRoom = new Room(Vector2.zero, Room.ERoomType.Start);
            m_rooms[m_gridSizeX, m_gridSizeY] = starterRoom;
            m_takenPositions.Insert(0, Vector2.zero);
            m_roomsList.Add(starterRoom);

            Vector2 positionToCheck;

            // Creating Regular Rooms
            for(int i = 0; i < numberOfRooms; i++) {
                // Getting random room to branch from
                Room roomToBranch = m_roomsList.RandomOrDefault();
                positionToCheck = GetPositionFromRoom(roomToBranch);

                if(positionToCheck == Vector2.zero) {
                    continue;
                }

                // Creating it as an Standard Room
                Room createdRoom = new Room(positionToCheck, Room.ERoomType.Regular);
                m_rooms[(int)positionToCheck.x + m_gridSizeX, (int)positionToCheck.y + m_gridSizeY] = createdRoom;

                m_roomsList.Add(createdRoom);
                createdRoom.SetParent(roomToBranch);
                roomToBranch.AddChildren(createdRoom);
                m_takenPositions.Insert(0, positionToCheck);
            }
        }

        private Vector2 GetPositionFromRoom(Room _room) {
            Vector2 position;
            int x, y;
            int maxTries = 15, currentTries = 0;

            do {
                currentTries++;

                if(currentTries >= maxTries) {
                    position = Vector2.zero;
                    break;
                }

                x = Mathf.RoundToInt(_room.gridPosition.x);
                y = Mathf.RoundToInt(_room.gridPosition.y);

                // deciding if we should branch up or down
                bool isUpOrDown = Random.value < 0.5f;
                bool isPositiveOrNegative = Random.value < 0.5f;

                if (isUpOrDown) {
                    if (isPositiveOrNegative) {
                        y++;
                    } else {
                        y--;
                    }
                } else {
                    if (isPositiveOrNegative) {
                        x++;
                    } else {
                        x--;
                    }
                }

                position = new Vector2(x, y);
            } while (
                    m_takenPositions.Contains(position) ||
                    x >= m_gridSizeX ||
                    x < -m_gridSizeX ||
                    y >= m_gridSizeY ||
                    y < -m_gridSizeY);

            return position;
        }
        #endregion ROOMS

        #region GOAL ROOM
        private void PlaceGoalRoom() {
            List<Room> possibleBossRooms = new List<Room>();
            List<Room> leafRooms = m_roomsList.Where(room => {
                return room.ChildrenRoomsAmount == 0;
            }).ToList();

            foreach(Room room in leafRooms) {
                Room parentRoom = room.ParentRoom;
                if(parentRoom.ChildrenRoomsAmount == 1) {
                    possibleBossRooms.Add(parentRoom);
                }
            }

            Debug.Log($"There are {possibleBossRooms.Count} possible boss rooms");
            // if there is no possible boss room just restart the generation
            if(possibleBossRooms.Count == 0) {
                Debug.Log($"Should abort Dungeon Generation!");
            } else {
                Room bossRoom = possibleBossRooms.RandomOrDefault();
                bossRoom.roomType = Room.ERoomType.BossRoom;
                
                // should have just one but using foreach just in case...
                foreach(Room childrenRoom in bossRoom.ChildrenRooms) {
                    childrenRoom.roomType = Room.ERoomType.GoalRoom;
                }
            }
        }
        #endregion

        #region VALIDATION
        private bool IsRoomValid() {
            int bossRoom = m_roomsList.Where(room => {
                return room.roomType == Room.ERoomType.BossRoom;
            }).Count();
            int goalRoom = m_roomsList.Where(room => {
                return room.roomType == Room.ERoomType.GoalRoom;
            }).Count();

            if(bossRoom == 1 && goalRoom == 1) {
                return true;
            }

            Debug.Log("Generated Room wasn't valid! Regenerating!");
            return false;
        }

        private void ClearLevel() {
            m_rooms = null;
            m_roomsList.Clear();
            m_roomInstanceList.Clear();
            m_takenPositions.Clear();
        }
        #endregion

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
                roomInstance.Setup(room.gridPosition, room.roomType, groundTilemap, groundRuleTile, wallTile, m_roomSizeInTiles);

                if(room.roomType == Room.ERoomType.BossRoom || room.roomType == Room.ERoomType.GoalRoom) {
                    bool doorUp = m_takenPositions.Contains(room.gridPosition + Vector2.up);
                    bool doorRight = m_takenPositions.Contains(room.gridPosition + Vector2.right);
                    bool doorDown = m_takenPositions.Contains(room.gridPosition + Vector2.down);
                    bool doorLeft = m_takenPositions.Contains(room.gridPosition + Vector2.left);

                    roomInstance.AddDoors(doorUp, doorRight, doorDown, doorLeft, doorPrefab);
                }
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

            // Adding Collision on Borders
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

            // Adding Collision for Wall Tiles
            if (tile == wallTile) {
                collisionTilemap.SetTile(new Vector3Int(_x, _y, 0), collisionTile);
                collisionTilemap.SetTile(new Vector3Int(_x, _y + 1, 0), collisionTile);
                collisionTilemap.SetTile(new Vector3Int(_x + 1, _y + 1, 0), collisionTile);
                collisionTilemap.SetTile(new Vector3Int(_x + 1, _y, 0), collisionTile);
                collisionTilemap.SetTile(new Vector3Int(_x + 1, _y - 1, 0), collisionTile);
                collisionTilemap.SetTile(new Vector3Int(_x, _y - 1, 0), collisionTile);
                collisionTilemap.SetTile(new Vector3Int(_x - 1, _y - 1, 0), collisionTile);
                collisionTilemap.SetTile(new Vector3Int(_x - 1, _y, 0), collisionTile);
                collisionTilemap.SetTile(new Vector3Int(_x - 1, _y + 1, 0), collisionTile);
            }

        }
        #endregion COLLISIONS

        #region OBJECTS
        private void InstantiateObjects() {
            // Instantiating Stairs to go to the city...
            Instantiate(townStairs, new Vector3(-0.5f, 0.5f, 0), Quaternion.identity);

            // Instantiate Boss Room Key anywhere in the dungeon
            RoomInstance randomRoom = m_roomInstanceList.Where(room => {
                return room.roomType == Room.ERoomType.Regular;
            }).ToList().RandomOrDefault();
            int positionX = Random.Range(2, (int)m_roomSizeInTiles.x - 2);
            int positionY = Random.Range(2, (int)m_roomSizeInTiles.y - 2);
            randomRoom.InstantiateObject(bossRoomKey, positionX, positionY);

            // Instantiate Goal Room Key on Boss Room
            RoomInstance bossRoom = m_roomInstanceList.Where(room => {
                return room.roomType == Room.ERoomType.BossRoom;
            }).First();

            if(bossRoom == null) {
                return;
            }

            positionX = Random.Range(2, (int)m_roomSizeInTiles.x - 2);
            positionY = Random.Range(2, (int)m_roomSizeInTiles.y - 2);
            bossRoom.InstantiateObject(bossEncounter, positionX, positionY);

            // Instantiating Stairs on the Goal Room
            RoomInstance goalRoom = m_roomInstanceList.Where(room => {
                return room.roomType == Room.ERoomType.GoalRoom;
            }).First();
            positionX = Random.Range(2, (int)m_roomSizeInTiles.x - 2);
            positionY = Random.Range(2, (int)m_roomSizeInTiles.y - 2);
            goalRoom.InstantiateObject(nextFloorStairs, positionX, positionY);

            // Instantiating Enemies
            RoomInstance[] regularRooms = m_roomInstanceList.Where(room => {
                return room.roomType == Room.ERoomType.Regular;
            }).ToArray();
            int amountOfEnemies = Random.Range(minimumAmountOfEnemies, maximumAmountOfEnemies);

            for(int i = 0; i < amountOfEnemies; i++) {
                positionX = Random.Range(1, (int)m_roomSizeInTiles.x - 1);
                positionY = Random.Range(1, (int)m_roomSizeInTiles.y - 1);
                regularRooms.RandomOrDefault().InstantiateObject(enemyEncounter, positionX, positionY);
            }
        }
        #endregion OBJECTS
    }
}
