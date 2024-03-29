﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeon {
    public class RoomInstance : MonoBehaviour {
        public Vector2 gridPosition;
        private Vector3 m_offset;
        public Room.ERoomType roomType;

        private Tilemap m_groundTilemap;
        private RuleTile m_groundTile;
        private RuleTile m_wallTile;
        private List<Vector3> m_tilePositionList;
        public List<Vector3> TilePositionList { get { return m_tilePositionList;  } }
        private Vector2 m_roomSizeInTiles;

        public void Setup(Vector2 _gridPosition, Room.ERoomType _roomType, Tilemap _groundTilemap, RuleTile _groundTile, RuleTile _wallTile, Vector2 _roomSizeInTiles) {
            m_tilePositionList = new List<Vector3>();

            this.gridPosition = _gridPosition;
            this.roomType = _roomType;

            m_groundTilemap = _groundTilemap;
            m_groundTile = _groundTile;
            m_wallTile = _wallTile;
            m_roomSizeInTiles = _roomSizeInTiles;

            m_offset = new Vector3((-m_roomSizeInTiles.x + 1),
                                    ((m_roomSizeInTiles.y / 4f)),
                                    0f);

            if(roomType == Room.ERoomType.Regular || roomType == Room.ERoomType.Start) {
                GenerateRoomTiles();
            } else {
                GenerateSpecialRoomTiles();
            }
        }

        private void GenerateRoomTiles() {
            for(int x = 0; x < m_roomSizeInTiles.x; x++) {
                for(int y = 0; y < m_roomSizeInTiles.y; y++) {
                    GenerateTile(x, y, m_groundTile);
                }
            }
        }

        private void GenerateSpecialRoomTiles() {
            for (int x = 0; x < (m_roomSizeInTiles.x); x++) {
                for (int y = 0; y < (m_roomSizeInTiles.y); y++) {
                    if(x == 0 || y == 0 || x == (m_roomSizeInTiles.x - 1) || y == (m_roomSizeInTiles.y - 1)) {
                        GenerateTile(x, y, m_wallTile);
                    } else {
                        GenerateTile(x, y, m_groundTile);
                    }
                }
            }
        }

        private void GenerateTile(int _x, int _y, RuleTile _tile) {
            Vector3 spawnPosition = PositionFromTileGrid(_x, _y);
            Vector3Int spawnPositionInt = new Vector3Int(Mathf.RoundToInt(spawnPosition.x), Mathf.RoundToInt(spawnPosition.y), 0);
            m_tilePositionList.Add(spawnPositionInt);
            m_groundTilemap.SetTile(spawnPositionInt, _tile);
        }

        private Vector3 PositionFromTileGrid(int _x, int _y) {
            Vector3 ret;
            ret = new Vector3(_x, -_y, 0);
            ret = ret + m_offset + transform.position;
            return ret;
        }

        public void AddDoors(bool _up, bool _right, bool _down, bool _left, GameObject _doorPrefab) {
            int middleWidth = Mathf.FloorToInt(m_roomSizeInTiles.x / 2);
            int middleHeight = Mathf.FloorToInt(m_roomSizeInTiles.y / 2);
            Vector3 doorOffset = new Vector3(0.5f, 0.5f, 0);

            Debug.Log($"Placing Doors: {_up} {_right} {_down} {_left}");

            if(_up) {
                PlaceDoorTile(middleWidth, 0);
                InstantiateDoor(_doorPrefab, PositionFromTileGrid(middleWidth, 0) + doorOffset);
            }

            if(_right) {
                PlaceDoorTile((int)m_roomSizeInTiles.x - 1, middleHeight);
                InstantiateDoor(_doorPrefab, PositionFromTileGrid((int)m_roomSizeInTiles.x - 1, middleHeight) + doorOffset);
            }

            if(_down) {
                PlaceDoorTile(middleWidth, ((int)m_roomSizeInTiles.y - 1));
                InstantiateDoor(_doorPrefab, PositionFromTileGrid(middleWidth, ((int)m_roomSizeInTiles.y - 1)) + doorOffset);
            }

            if(_left) {
                PlaceDoorTile(0, middleHeight);
                InstantiateDoor(_doorPrefab, PositionFromTileGrid(0, middleHeight) + doorOffset);
            }
        }

        private void PlaceDoorTile(int _x, int _y) {
            GenerateTile(_x, _y, m_groundTile);

            GenerateTile(_x, _y + 1, m_groundTile);
            GenerateTile(_x + 1, _y + 1, m_groundTile);
            GenerateTile(_x + 1, _y, m_groundTile);
            GenerateTile(_x + 1, _y - 1, m_groundTile);
            GenerateTile(_x, _y - 1, m_groundTile);
            GenerateTile(_x - 1, _y - 1, m_groundTile);
            GenerateTile(_x - 1, _y, m_groundTile);
            GenerateTile(_x - 1, _y + 1, m_groundTile);
        }

        private void InstantiateDoor(GameObject _prefab, Vector3 _position) {
            Door instantiatedDoor = Instantiate(_prefab, _position, Quaternion.identity).GetComponent<Door>();

            if (roomType == Room.ERoomType.BossRoom) {
                instantiatedDoor.doorType = Door.EDoorType.BossRoom;
            } else if(roomType == Room.ERoomType.GoalRoom) {
                instantiatedDoor.doorType = Door.EDoorType.GoalRoom;
            }
        }

        public void InstantiateObject(GameObject _prefab, int _x, int _y) {
            Vector3 position = PositionFromTileGrid(_x, _y);
            Instantiate(_prefab, position + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
        }
    }
}
