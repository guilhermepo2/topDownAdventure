using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeon {
    public class RoomInstance : MonoBehaviour {
        public Vector2 gridPosition;
        public Vector2 roomSizeInTiles = new Vector2(9, 17);
        private Vector3 m_offset;
        public Room.ERoomType roomType;

        private Tilemap m_groundTilemap;
        private RuleTile m_groundTile;
        private RuleTile m_wallTile;
        private List<Vector3> m_tilePositionList;
        public List<Vector3> TilePositionList { get { return m_tilePositionList;  } }

        public void Setup(Vector2 _gridPosition, Room.ERoomType _roomType, Tilemap _groundTilemap, RuleTile _groundTile, RuleTile _wallTile) {
            m_tilePositionList = new List<Vector3>();

            this.gridPosition = _gridPosition;
            this.roomType = _roomType;

            m_groundTilemap = _groundTilemap;
            m_groundTile = _groundTile;
            m_wallTile = _wallTile;

            m_offset = new Vector3((-roomSizeInTiles.x + 1),
                                    ((roomSizeInTiles.y / 4f)),
                                    0f);

            if(roomType == Room.ERoomType.Regular || roomType == Room.ERoomType.Start) {
                GenerateRoomTiles();
            } else {
                GenerateSpecialRoomTiles();
            }
        }

        private void GenerateRoomTiles() {
            for(int x = 0; x < roomSizeInTiles.x; x++) {
                for(int y = 0; y < roomSizeInTiles.y; y++) {
                    GenerateTile(x, y, m_groundTile);
                }
            }
        }

        private void GenerateSpecialRoomTiles() {
            for (int x = 0; x < (roomSizeInTiles.x); x++) {
                for (int y = 0; y < (roomSizeInTiles.y); y++) {
                    if(x == 0 || y == 0 || x == (roomSizeInTiles.x - 1) || y == (roomSizeInTiles.y - 1)) {
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

        public void AddDoors(bool _up, bool _right, bool _down, bool _left) {
            int middleWidth = Mathf.FloorToInt(roomSizeInTiles.x / 2);
            int middleHeight = Mathf.FloorToInt(roomSizeInTiles.y / 2);

            Debug.Log($"Placing Doors: {_up} {_right} {_down} {_left}");

            if(_up) {
                PlaceDoorTile(middleWidth, 0);
            }

            if(_right) {
                PlaceDoorTile((int)roomSizeInTiles.x - 1, middleHeight);
            }

            if(_down) {
                PlaceDoorTile(middleWidth, ((int)roomSizeInTiles.y - 1));
            }

            if(_left) {
                PlaceDoorTile(0, middleHeight);
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
    }
}
