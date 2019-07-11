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
        private Tile m_groundTile;

        public void Setup(Vector2 _gridPosition, Room.ERoomType _roomType, Tilemap _groundTilemap, Tile _groundTile) {
            this.gridPosition = _gridPosition;
            this.roomType = _roomType;

            m_groundTilemap = _groundTilemap;
            m_groundTile = _groundTile;

            m_offset = new Vector3((-roomSizeInTiles.x + 1),
                                    ((roomSizeInTiles.y / 4f)),
                                    0f);
            GenerateRoomTiles();
        }

        private void GenerateRoomTiles() {
            for(int x = 0; x < roomSizeInTiles.x; x++) {
                for(int y = 0; y < roomSizeInTiles.y; y++) {
                    GenerateTile(x, y);
                }
            }
        }

        private void GenerateTile(int _x, int _y) {
            Vector3 spawnPosition = PositionFromTileGrid(_x, _y);
            Vector3Int spawnPositionInt = new Vector3Int(Mathf.RoundToInt(spawnPosition.x), Mathf.RoundToInt(spawnPosition.y), 0);
            m_groundTilemap.SetTile(spawnPositionInt, m_groundTile);
        }

        private Vector3 PositionFromTileGrid(int _x, int _y) {
            Vector3 ret;
            ret = new Vector3(_x, -_y, 0);
            ret = ret + m_offset + transform.position;
            return ret;
        }
    }
}
